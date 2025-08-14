namespace CleanArchitecture.Domain.Shared.Pagination;

/// <summary>
/// Sayfalama sonucuna ait meta bilgiler (UI'nin navigasyon/indikator için ihtiyacı olanlar).
/// Not: Bu tip yalnızca hesap/taşıma yapar; veri erişimine bağımlı değildir.
/// </summary>
/// <remarks>
/// Varsayımlar:
/// - PageNumber 1 tabanlıdır (ilk sayfa = 1).
/// - PageSize > 0 olmalıdır (Normalize aşamasında garanti edilir).
/// - TotalCount null ise toplam sayım yapılmamıştır (IncludeTotalCount=false).
/// </remarks>
public class PageMeta
{
    /// <summary>
    /// Mevcut sayfa numarası (1,2,3...).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Sayfa başına kayıt adedi.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Toplam kayıt sayısı. 
    /// Null ise toplam sayım yapılmamış demektir (performans için opsiyoneldir).
    /// </summary>
    public int? TotalCount { get; init; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// TotalCount bilinmiyorsa (null) bu da null döner.
    /// </summary>
    /// <remarks>
    /// Math.Ceiling: örn. 23 kayıt, PageSize=10 => 2.3 → 3 sayfa.
    /// PageSize'ın 0 olma ihtimali normalize ile engellenmiş kabul edilir.
    /// </remarks>
    public int? TotalPages => TotalCount is null
        ? (int?)null
        : (int)Math.Ceiling((double)TotalCount.Value / PageSize);

    /// <summary>
    /// Önceki sayfa var mı? (PageNumber>1 ise true)
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Sonraki sayfa var mı?
    /// TotalCount bilinmiyorsa (null) bu bilgi de bilinemez → null.
    /// </summary>
    /// <remarks>
    /// TotalPages null değilse, mevcut sayfanın son sayfadan küçük olup olmadığına bakarız.
    /// </remarks>
    public bool? HasNext => TotalCount is null
        ? (bool?)null
        : PageNumber < TotalPages;
}
