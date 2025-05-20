using System.Diagnostics.CodeAnalysis;

namespace UserService.Domain.Results;

/// <summary>
///     Represents the base result of an operation, indicating success or failure and containing optional error
///     information.
/// </summary>
public class BaseResult
{
    protected BaseResult()
    {
    }

    protected BaseResult(string errorMessage, int? errorCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    /// <summary>
    ///     Indicates whether the operation was successful. Returns true if <see cref="ErrorMessage" /> is <c>null</c> .
    /// </summary>
    public bool IsSuccess => ErrorMessage == null;

    /// <summary>
    ///     An optional error message that describes why the operation failed.
    ///     If this value is <c>null</c>, the operation is considered successful.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    ///     An optional numeric code representing the type of error.
    /// </summary>
    public int? ErrorCode { get; }


    /// <summary>
    ///     Creates a successful result without any additional data.
    /// </summary>
    /// <returns>A successful <see cref="BaseResult" /> instance.</returns>
    public static BaseResult Success()
    {
        return new BaseResult();
    }

    /// <summary>
    ///     Creates a failed result with a specified error message and optional error code.
    /// </summary>
    /// <param name="errorMessage">A non-empty error message.</param>
    /// <param name="errorCode">An optional numeric error code.</param>
    /// <returns>A failed <see cref="BaseResult" /> instance containing the specified error.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessage" /> is <c>null</c> or whitespace.</exception>
    public static BaseResult Failure(string errorMessage, int? errorCode = null)
    {
        return new BaseResult(errorMessage, errorCode);
    }
}

/// <summary>
///     Represents the base result of an operation that returns a value of type <typeparamref name="T" />.
///     Inherits from <see cref="BaseResult" />.
/// </summary>
/// <typeparam name="T">The type of data returned when the operation is successful. Must be a reference type.</typeparam>
public class BaseResult<T> : BaseResult where T : class
{
    protected BaseResult(T data)
    {
        ArgumentNullException.ThrowIfNull(data);
        Data = data;
    }

    protected BaseResult(string errorMessage, int? errorCode = null)
        : base(errorMessage, errorCode)
    {
    }

    /// <inheritdoc cref="BaseResult.IsSuccess" />
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool IsSuccess => base.IsSuccess;

    /// <summary>
    ///     The data returned by the operation if successful; <c>null</c> otherwise.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    ///     Creates a successful result with the specified data.
    /// </summary>
    /// <param name="data">The data to return. Cannot be <c>null</c>.</param>
    /// <returns>A successful <see cref="BaseResult{T}" /> containing the data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <c>null</c> or whitespace.</exception>
    public static BaseResult<T> Success(T data)
    {
        return new BaseResult<T>(data);
    }

    /// <inheritdoc cref="BaseResult.Failure" />
    /// <returns>A failed <see cref="BaseResult{T}" /> instance containing the specified error.</returns>
    public new static BaseResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new BaseResult<T>(errorMessage, errorCode);
    }
}