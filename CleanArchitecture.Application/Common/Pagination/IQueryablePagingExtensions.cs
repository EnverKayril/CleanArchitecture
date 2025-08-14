using CleanArchitecture.Domain.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanArchitecture.Application.Common.Pagination;

/// <summary>
/// IQueryable için sayfalama (paging) uzantıları.
/// - Projection'lı (DTO'ya Select) ve projection'sız (entity) overload'lar içerir.
/// - Deterministik sıralama uygular (SortBy yoksa default, o da yoksa "Id").
/// - TotalCount istenirse 2 sorgu (COUNT + page), istenmezse tek sorgu (over-fetch) çalışır.
/// Not: AsNoTracking çağrısı bu uzantılarda değil, çağıran tarafta yapılmalıdır.
/// </summary>
public static class IQueryablePagingExtensions
{
    // --------------------------------------------------------------------
    // 1) PROJECTION'LI overload: IQueryable<T> -> IQueryable<TDest> (Select ile)
    // --------------------------------------------------------------------
    public static async Task<PaginatedResult<TDest>> ToPaginatedResultAsync<T, TDest>(
        this IQueryable<T> query,                     // EF'nin SQL'e çevireceği sorgu ağacı
        PagedRequest request,                         // sayfa/sıralama/Count isteği
        Expression<Func<T, TDest>> selector,          // sadece gerekli kolonları çeken projeksiyon
        CancellationToken ct = default,               // iptal desteği
        IEnumerable<string>? sortableWhitelist = null,// sıralamaya izin verilen alanlar (güvenlik/sağlamlık)
        string? defaultSortBy = null,                 // SortBy boşsa kullanılacak default alan
        bool defaultDesc = false)                     // default alan için sıralama yönü
    {
        // 0) Güvenli aralıklara çek (örn. PageSize 100'ü aşmasın)
        var (pageNumber, pageSize) = PaginationOptions.Normalize(request.PageNumber, request.PageSize);

        // 1) Deterministik sıralama: paging stabil çalışsın
        if (!string.IsNullOrWhiteSpace(request.SortBy))
            query = query.OrderBySafe(request.SortBy, request.Desc, sortableWhitelist);
        else if (!string.IsNullOrWhiteSpace(defaultSortBy))
            query = query.OrderBySafe(defaultSortBy, defaultDesc, sortableWhitelist);
        else
            // "Id" her entity'de olmayabilir; yoksa çağıran defaultSortBy vermelidir.
            query = query.OrderBySafe("Id", false, sortableWhitelist);

        if (request.IncludeTotalCount)
        {
            // 2A) Klasik mod: Toplam kayıt sayısını da iste
            var total = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)   // sayfa kaydır
                .Take(pageSize)                      // sayfa kadar getir
                .Select(selector)                    // sadece gereken kolonlar
                .ToListAsync(ct);                    // materialize (SQL burada çalışır)

            return new PaginatedResult<TDest>
            {
                Items = items,
                Meta = new PageMeta
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total               // TotalPages/HasNext hesaplanabilir
                }
            };
        }
        else
        {
            // 2B) Performans mod: Tek sorgu ile "bir sonraki var mı?"yı anlamak için over-fetch
            var pagePlusOne = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize + 1)                  // bir fazla getir
                .Select(selector)
                .ToListAsync(ct);

            var hasNext = pagePlusOne.Count > pageSize; // fazladan geldi mi? geldiyse bir sonraki sayfa var
            var items = hasNext ? pagePlusOne.Take(pageSize).ToList()
                                : pagePlusOne;

            // Not: PageMeta.HasNext, TotalCount null olduğunda null döner (tasarım gereği).
            // "hasNext" bilgisini cevapta istiyorsan meta modeline ayrı bir alan ekleyebilirsin (örn. HasNextProbe).
            return new PaginatedResult<TDest>
            {
                Items = items,
                Meta = new PageMeta
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = null                // toplam sayım bilinçli olarak yapılmadı
                }
            };
        }
    }

    // --------------------------------------------------------------------
    // 2) PROJECTION'SIZ overload: IQueryable<T> -> T (entity listesi döner)
    // --------------------------------------------------------------------
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query,
        PagedRequest request,
        CancellationToken ct = default,
        IEnumerable<string>? sortableWhitelist = null,
        string? defaultSortBy = null,
        bool defaultDesc = false)
    {
        var (pageNumber, pageSize) = PaginationOptions.Normalize(request.PageNumber, request.PageSize);

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            query = query.OrderBySafe(request.SortBy, request.Desc, sortableWhitelist);
        else if (!string.IsNullOrWhiteSpace(defaultSortBy))
            query = query.OrderBySafe(defaultSortBy, defaultDesc, sortableWhitelist);
        else
            query = query.OrderBySafe("Id", false, sortableWhitelist);

        if (request.IncludeTotalCount)
        {
            var total = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<T>
            {
                Items = items,
                Meta = new PageMeta
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total
                }
            };
        }
        else
        {
            var pagePlusOne = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize + 1)
                .ToListAsync(ct);

            var hasNext = pagePlusOne.Count > pageSize;
            var items = hasNext ? pagePlusOne.Take(pageSize).ToList()
                                : pagePlusOne;

            return new PaginatedResult<T>
            {
                Items = items,
                Meta = new PageMeta
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = null
                }
            };
        }
    }

    public static Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
    this IQueryable<T> query,
    int pageNumber, int pageSize,
    CancellationToken ct = default,
    IEnumerable<string>? sortableWhitelist = null,
    string? defaultSortBy = null,
    bool defaultDesc = false,
    string? sortBy = null,
    bool desc = false,
    bool includeTotalCount = true)
    {
        var req = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            Desc = desc,
            IncludeTotalCount = includeTotalCount
        };
        return query.ToPaginatedResultAsync(req, ct, sortableWhitelist, defaultSortBy, defaultDesc);
    }
}
