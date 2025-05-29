using System.Diagnostics.CodeAnalysis;

namespace UserService.Domain.Results;

/// <summary>
///     Represents the result of an operation returning a queryable collection of items of type <typeparamref name="T" />.
///     Inherits from <see cref="BaseResult{T}" /> where <typeparamref name="T" /> is <see cref="IQueryable{T}" />.
/// </summary>
/// <typeparam name="T">The type of items in the queryable result.</typeparam>
public class QueryableResult<T> : BaseResult<IQueryable<T>>
{
    // IQueryable<T> is not serializable and should not be exposed externally.
    // Therefore, this class does not include a parameterless constructor.

    protected QueryableResult(IQueryable<T> data) : base(data)
    {
    }

    protected QueryableResult(string errorMessage, int? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    /// <inheritdoc cref="BaseResult.IsSuccess" />
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool IsSuccess => base.IsSuccess;

    /// <inheritdoc cref="BaseResult{T}.Data" />
    public new IQueryable<T>? Data => base.Data;

    /// <summary>
    ///     Creates a successful <see cref="QueryableResult{T}" /> with the specified <see cref="IQueryable{T}" /> data source.
    /// </summary>
    /// <param name="data">The query to <typeparamref name="T" /> datasource to return. Cannot be <c>null</c>.</param>
    /// <returns>A successful <see cref="QueryableResult{T}" /> containing the query.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c>.</exception>
    public new static QueryableResult<T> Success(IQueryable<T> data)
    {
        return new QueryableResult<T>(data);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="QueryableResult{T}" /> instance containing the specified error.</returns>
    public new static QueryableResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new QueryableResult<T>(errorMessage, errorCode);
    }
}