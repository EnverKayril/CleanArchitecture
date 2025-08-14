namespace CleanArchitecture.Domain.Shared.Pagination;

/// <summary>
/// Sayfalama/sıralama istek parametreleri için ortak taşıyıcı (input DTO).
/// Amaç: Her listeleme uç noktasında aynı sözleşmeyi kullanmak.
/// Not: Bu tip Domain'de tutulur; EF Core'a veya başka kütüphaneye bağımlı değildir.
/// </summary>
/// <remarks>
/// Neden record?
/// - C# 9 'record', değer-semantikli ve immutability dostu bir tiptir.
/// - 'init' setter'ları ile nesne oluşturulduktan sonra değerlerin değiştirilmesini engelleriz.
/// - Bu, istek parametrelerinin akış içinde yanlışlıkla değiştirilmesini önler.
/// </remarks>
public record PagedRequest
{
    /// <summary>
    /// İstenen sayfa numarası. 1 tabanlıdır (ilk sayfa = 1).
    /// Varsayılan: 1
    /// </summary>
    /// <remarks>
    /// Buradaki değer ham olarak taşınır; güvenli aralık kontrolü
    /// Application katmanında PaginationOptions.Normalize ile yapılır.
    /// </remarks>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Sayfa başına kayıt adedi.
    /// Varsayılan: 10
    /// </summary>
    /// <remarks>
    /// Üst sınır (ör. 100) yine normalize aşamasında uygulanır.
    /// Domain seviyesinde sadece sözleşmeyi tanımlarız.
    /// </remarks>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Sıralanacak sütun adı (örn: "Name", "CreatedDate").
    /// Boş veya null ise çağıran katman varsayılan bir kolon seçecektir.
    /// </summary>
    /// <remarks>
    /// Güvenlik/sağlamlık için dinamik sıralamada beyaz liste kontrolü
    /// (allowedColumns) Application tarafındaki OrderBySafe içinde yapılır.
    /// </remarks>
    public string? SortBy { get; init; } = null;

    /// <summary>
    /// true ise azalan (DESC), false ise artan (ASC) sıralama yapılır.
    /// Varsayılan: false (artan)
    /// </summary>
    public bool Desc { get; init; } = false;

    /// <summary>
    /// Toplam kayıt sayısı hesaplansın mı?
    /// true:  COUNT + sayfa verisi (2 sorgu) → UI toplam sayfa vb. gösterir.
    /// false: Sadece sayfa verisi (tek sorgu, daha performanslı).
    /// </summary>
    /// <remarks>
    /// Büyük tablolarda performans için false tercih edilebilir.
    /// Bu durumda PageMeta.TotalCount null döner.
    /// </remarks>
    public bool IncludeTotalCount { get; init; } = true;
}
