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

    protected PageResult(IEnumerable<T> data, int pageNumber) : base(data)
    {
        if (pageNumber <= 0)
            // ReSharper disable once LocalizableElement
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

        PageNumber = pageNumber;
    }

    protected PageResult(string errorMessage, int? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    /// <summary>
    ///     The current page number in the paginated result.
    /// </summary>
    [JsonProperty]
    public int PageNumber { get; private init; }

    /// <summary>
    ///     Creates a successful <see cref="PageResult{T}" /> with the specified collection of <typeparamref name="T" /> items
    ///     and page information.
    /// </summary>
    /// <param name="data">The collection of <typeparamref name="T" /> items to return. Cannot be <c>null</c>.</param>
    /// <param name="pageNumber">The current page number. Must be a positive integer.</param>
    /// <returns>A successful <see cref="PageResult{T}" /> containing the data and page information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber" /> is less than or equal to zero.</exception>
    public static PageResult<T> Success(IEnumerable<T> data, int pageNumber)
    {
        return new PageResult<T>(data, pageNumber);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="PageResult{T}" /> instance containing the specified error.</returns>
    public new static PageResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new PageResult<T>(errorMessage, errorCode);
    }
}