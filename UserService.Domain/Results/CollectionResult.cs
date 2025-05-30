using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace UserService.Domain.Results;

/// <summary>
///     Represents the result of an operation returning a collection of items of type <typeparamref name="T" />.
///     Inherits from <see cref="BaseResult{T}" /> where <typeparamref name="T" /> is <see cref="IEnumerable{T}" />>.
/// </summary>
/// <typeparam name="T">The type of items in the returned collection.</typeparam>
public class CollectionResult<T> : BaseResult<IEnumerable<T>>
{
    [JsonConstructor]
    protected CollectionResult()
    {
    }

    protected CollectionResult(IEnumerable<T> data) : base(data)
    {
    }

    protected CollectionResult(string errorMessage, int? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    /// <inheritdoc cref="BaseResult.IsSuccess" />
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool IsSuccess => base.IsSuccess;

    /// <inheritdoc cref="BaseResult{T}.Data" />
    public new IEnumerable<T>? Data => base.Data;

    /// <summary>
    ///     The number of items returned in the current collection.
    ///     Returns <c>0</c> if the <see cref="BaseResult{T}.Data" /> collection is <c>null</c>.
    /// </summary>
    public int Count => Data?.Count() ?? 0;

    /// <summary>
    ///     Creates a successful <see cref="CollectionResult{T}" /> with the specified collection of <typeparamref name="T" />
    ///     items.
    /// </summary>
    /// <param name="data">The collection of <typeparamref name="T" /> items to return. Cannot be <c>null</c>.</param>
    /// <returns>A successful <see cref="CollectionResult{T}" /> containing the data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c>.</exception>
    public new static CollectionResult<T> Success(IEnumerable<T> data)
    {
        return new CollectionResult<T>(data);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="CollectionResult{T}" /> instance containing the specified error.</returns>
    public new static CollectionResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new CollectionResult<T>(errorMessage, errorCode);
    }
}