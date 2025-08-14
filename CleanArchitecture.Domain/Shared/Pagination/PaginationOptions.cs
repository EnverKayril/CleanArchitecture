namespace CleanArchitecture.Domain.Shared.Pagination;

/// <summary>
/// Sayfalama ile ilgili sistem genelinde geçerli olacak sabitler
/// ve normalize işlemini barındırır.
/// </summary>
public class PaginationOptions
{
    /// <summary>
    /// İzin verilen en küçük sayfa numarası.
    /// 1 olarak tutulur çünkü sayfalama genelde 1 tabanlıdır (0 değil).
    /// </summary>
    public const int MinPageNumber = 1;

    /// <summary>
    /// İzin verilen en küçük sayfa boyutu (kaç kayıt getirileceği).
    /// Performans açısından 0 veya negatif değer olmaması gerekir.
    /// </summary>
    public const int MinPageSize = 1;

    /// <summary>
    /// İzin verilen en büyük sayfa boyutu.
    /// Çok büyük sayfa boyutlarının sistemi yormaması için limit konur.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Dışarıdan gelen pageNumber ve pageSize değerlerini normalize eder.
    /// Yani sistemdeki kurallara göre minimum/maximum aralıklara çeker.
    /// </summary>
    /// <param name="pageNumber">İstenen sayfa numarası.</param>
    /// <param name="pageSize">İstenen sayfa boyutu.</param>
    /// <returns>
    /// Tuple (pageNumber, pageSize) — ikisi de normalize edilmiş değerler.
    /// </returns>
    public static (int pageNumber, int pageSize) Normalize(int pageNumber, int pageSize)
    {
        // Sayfa numarası minimumdan küçükse minimuma çek
        if (pageNumber < MinPageNumber) pageNumber = MinPageNumber;

        // Sayfa boyutu minimumdan küçükse minimuma çek
        if (pageSize < MinPageSize) pageSize = MinPageSize;

        // Sayfa boyutu maximumdan büyükse maximuma çek
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        // (pageNumber, pageSize) tuple olarak dön
        return (pageNumber, pageSize);
    }
}
