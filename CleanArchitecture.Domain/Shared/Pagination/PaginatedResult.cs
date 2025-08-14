namespace CleanArchitecture.Domain.Shared.Pagination;

/// <summary>
/// Sayfalama sonucu için standart dönüş modeli.
/// Hem veri listesini hem de sayfa meta bilgisini taşır.
/// </summary>
/// <typeparam name="T">
/// Liste elemanlarının tipi (ör: Car, Product, UserDto).
/// </typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// İlgili sayfadaki veri listesi.
    /// IReadOnlyList: dışarıdan değiştirilemesin diye kullanıyoruz (immutable koleksiyon).
    /// Varsayılan olarak boş bir dizi ile başlatılır.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Sayfalama meta bilgisi (mevcut sayfa numarası, toplam sayfa sayısı, vb.).
    /// Varsayılan olarak boş bir PageMeta ile başlatılır.
    /// </summary>
    public PageMeta Meta { get; init; } = new();
}
