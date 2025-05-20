using System.Diagnostics.CodeAnalysis;

namespace UserService.Domain.Results;

/// <summary>
///     Represents the result of an operation returning a collection of items of type <typeparamref name="T" />.
///     Inherits from <see cref="BaseResult{T}" /> where <typeparamref name="T" /> is <see cref="IEnumerable{T}" />>.
/// </summary>
/// <typeparam name="T">The type of items in the returned collection.</typeparam>
public class CollectionResult<T> : BaseResult<IEnumerable<T>>
{
    protected CollectionResult(IEnumerable<T> data, int? totalCount = null) : base(data)
    {
        if (totalCount < Count)
            // ReSharper disable once LocalizableElement
            throw new ArgumentException($"{nameof(totalCount)} cannot be less than {nameof(Count)}.",
                nameof(totalCount));

        TotalCount = totalCount;
    }

    protected CollectionResult(string errorMessage, int? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    /// <inheritdoc cref="BaseResult.IsSuccess" />
    [MemberNotNullWhen(true, nameof(Count))]
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool IsSuccess => base.IsSuccess;

    /// <inheritdoc cref="BaseResult{T}.Data" />
    public new IEnumerable<T>? Data => base.Data;

    /// <summary>
    ///     The number of items returned in the current collection.
    ///     Returns <c>0</c> if the <see cref="BaseResult{T}.Data" /> collection is empty.
    /// </summary>
    public int Count => Data?.Count() ?? 0;

    /// <summary>
    ///     The total number of items available, which may be greater than <see cref="Count" />.
    ///     Can be <c>null</c>.
    /// </summary>
    public int? TotalCount { get; }

    /// <summary>
    ///     Creates a successful <see cref="CollectionResult{T}" /> with the specified collection of <typeparamref name="T" />
    ///     items.
    /// </summary>
    /// <param name="data">The collection of <typeparamref name="T" /> items to return. Cannot be <c>null</c>.</param>
    /// <param name="totalCount">The optional total number of available <typeparamref name="T" /> items.</param>
    /// <returns>A successful <see cref="CollectionResult{T}" /> containing the data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c> or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="totalCount" /> is less than <see cref="Count" />.</exception>
    public static CollectionResult<T> Success(IEnumerable<T> data, int? totalCount = null)
    {
        return new CollectionResult<T>(data, totalCount);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="CollectionResult{T}" /> instance containing the specified error.</returns>
    public new static CollectionResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new CollectionResult<T>(errorMessage, errorCode);
    }
}