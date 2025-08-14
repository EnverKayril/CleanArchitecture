using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Pagination;

/// <summary>
/// IQueryable için dinamik (string tabanlı) sıralama uzantısı.
/// Amaç: "Name", "CreatedDate" gibi kolon adları runtime'da string olarak geldiğinde
/// EF'nin anlayacağı Expression ağacını kurup OrderBy/OrderByDescending çağırmak.
/// </summary>
public static class IQueryableOrderingExtensions
{
    /// <summary>
    /// "sortBy" kolon adına göre güvenli (whitelist destekli) sıralama uygular.
    /// - sortBy boşsa: sorguya dokunmaz (erken çıkar).
    /// - allowedColumns verilmişse: listedekiler dışındakileri reddeder (erken çıkar).
    /// - Kolon adı tipte yoksa: dokunmadan geri döner (erken çıkar).
    /// </summary>
    /// <typeparam name="T">Sorgudaki öğe tipi (entity/DTO vs.)</typeparam>
    /// <param name="source">Sıralanacak IQueryable</param>
    /// <param name="sortBy">Sıralanacak kolon adı (örn: "Name")</param>
    /// <param name="desc">true: DESC, false: ASC</param>
    /// <param name="allowedColumns">Opsiyonel beyaz liste (güvenlik/sağlamlık)</param>
    /// <returns>Sıralama uygulanmış ya da değiştirilmemiş IQueryable</returns>
    public static IQueryable<T> OrderBySafe<T>(
        this IQueryable<T> source,
        string? sortBy,
        bool desc,
        IEnumerable<string>? allowedColumns = null)
    {
        // 1) sortBy boş/whitespace ise hiç elleme (sıralama istenmemiş say)
        if (string.IsNullOrWhiteSpace(sortBy))
            return source;

        // 2) allowedColumns verildiyse "case-insensitive" kontrol yapalım:
        //    Aksi halde "name" ile "Name" uyuşmazlık çıkarabilir.
        if (allowedColumns is not null)
        {
            // HashSet ile O(1) lookup ve case-insensitive kıyas
            var white = allowedColumns is HashSet<string> h
                ? h
                : new HashSet<string>(allowedColumns, StringComparer.OrdinalIgnoreCase);

            if (!white.Contains(sortBy))
                return source; // izin yoksa sessizce dokunmadan çık
        }

        // 3) "sortBy" gerçekten T tipinde bir property/field mı? (güvenlik ve sağlamlık)
        //    PropertyOrField ikisini de kapsıyor ama önce reflection ile var mı bakalım:
        var member = (MemberInfo?)typeof(T).GetProperty(sortBy!,
                              BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                    ?? typeof(T).GetField(sortBy!,
                              BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (member is null)
            return source; // yoksa sıralama yapma, sessizce geri dön

        // 4) Expression ağacını kur:
        //    x => x.<PropertyOrField>
        var param = Expression.Parameter(typeof(T), "x");      // "x"
        Expression body;

        // Property ise Property, field ise Field expression üret
        if (member is PropertyInfo pi)
            body = Expression.Property(param, pi);             // x.<Prop>
        else
            body = Expression.Field(param, (FieldInfo)member); // x.<Field>

        // Lambda: x => x.<Name>
        var keySelector = Expression.Lambda(body, param);

        // 5) Doğru Queryable.* metodu (OrderBy / OrderByDescending) yakala
        var methodName = desc ? "OrderByDescending" : "OrderBy";

        // Queryable'daki generic 2 parametreli (source, keySelector) overload'ını bul
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName
                     && m.GetParameters().Length == 2);

        // Generic argümanları (T, TKey) bağla:
        // - T: sorgunun öğe tipi
        // - TKey: anahtarın gerçek tipi (string, int, DateTime...) -> body.Type
        var generic = method.MakeGenericMethod(typeof(T), body.Type);

        // 6) Statik metodu Invoke et: OrderBy<T, TKey>(source, keySelector)
        //    Dönüş tipi IOrderedQueryable<T> ama IQueryable<T> olarak da iş görür.
        var ordered = generic.Invoke(null, new object[] { source, keySelector })!;

        return (IQueryable<T>)ordered;
    }
}
