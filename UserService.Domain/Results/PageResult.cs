using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace UserService.Domain.Results;

/// <summary>
///     Represents the result of a paginated collection operation returning a collection of items of type
///     <typeparamref name="T" />.
///     Inherits from <see cref="PageResult{T}" /> and adds information about the current page.
/// </summary>
/// <typeparam name="T">The type of items in the returned collection.</typeparam>
public class PageResult<T> : CollectionResult<T>
{
    // Constructor for JSON deserialization
    protected PageResult()
    {
    }

    [SuppressMessage("ReSharper", "LocalizableElement")]
    protected PageResult(IEnumerable<T> data, int pageNumber, int totalCount) : base(data)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), Resources.ErrorMessage.InvalidPageNumber);
        if (totalCount < Count)
            throw new ArgumentException($"{nameof(totalCount)} cannot be less than {nameof(Count)}.",
                nameof(totalCount));

        TotalCount = totalCount;
        PageNumber = pageNumber;
    }

    protected PageResult(string errorMessage, int? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    /// <inheritdoc cref="CollectionResult{T}.IsSuccess" />
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool IsSuccess => base.IsSuccess;

    /// <inheritdoc cref="CollectionResult{T}.Data" />
    public new IEnumerable<T>? Data => base.Data;

    /// <summary>
    ///     The current page number in the paginated result.
    /// </summary>
    [JsonProperty]
    public int PageNumber { get; private init; }

    /// <summary>
    ///     The total number of items available, which may be greater than <see cref="CollectionResult{T}.Count" />.
    /// </summary>
    [JsonProperty]
    public int TotalCount { get; private init; }

    /// <summary>
    ///     Creates a successful <see cref="PageResult{T}" /> with the specified collection of <typeparamref name="T" /> items
    ///     and page information.
    /// </summary>
    /// <param name="data">The collection of <typeparamref name="T" /> items to return. Cannot be <c>null</c>.</param>
    /// <param name="pageNumber">The current page number. Must be a positive integer.</param>
    /// <param name="totalCount">The total number of available <typeparamref name="T" /> items.</param>
    /// <returns>A successful <see cref="PageResult{T}" /> containing the data and page information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber" /> is less than or equal to zero.</exception>
    /// /// <exception cref="ArgumentException">Thrown when <paramref name="totalCount" /> is less than <see cref="CollectionResult{T}.Count"/>.</exception>
    public static PageResult<T> Success(IEnumerable<T> data, int pageNumber, int totalCount)
    {
        return new PageResult<T>(data, pageNumber, totalCount);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="PageResult{T}" /> instance containing the specified error.</returns>
    public new static PageResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new PageResult<T>(errorMessage, errorCode);
    }
}