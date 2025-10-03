using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Enums;
using UserService.Domain.Results;

namespace UserService.Api.Controllers.Base;

/// <inheritdoc />
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class BaseController : ControllerBase
{
    private static readonly IReadOnlyDictionary<int, int> ErrorStatusCodeMap = new Dictionary<int, int>
    {
        // Reputation
        { (int)ErrorCodes.DailyReputationLimitExceeded, StatusCodes.Status429TooManyRequests },
        { (int)ErrorCodes.ReputationMinimumReached, StatusCodes.Status400BadRequest },
        { (int)ErrorCodes.CannotIncreaseOrDecreaseNegativeReputation, StatusCodes.Status400BadRequest },

        // User
        { (int)ErrorCodes.UserNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.UserAlreadyExists, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.UserAlreadyHasThisRole, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.UsersNotFound, StatusCodes.Status404NotFound },

        // Authorization
        { (int)ErrorCodes.PasswordIsWrong, StatusCodes.Status401Unauthorized },
        { (int)ErrorCodes.EmailNotValid, StatusCodes.Status400BadRequest },
        { (int)ErrorCodes.InvalidToken, StatusCodes.Status400BadRequest },

        // Roles
        { (int)ErrorCodes.RoleAlreadyExists, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.RoleNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.RolesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.CannotDeleteDefaultRole, StatusCodes.Status403Forbidden },

        // Validity
        { (int)ErrorCodes.InvalidPagination, StatusCodes.Status400BadRequest }
    };

    /// <summary>
    ///     Handles the BaseResult of type T and returns the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    protected ActionResult<BaseResult<T>> HandleBaseResult<T>(BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        var statusCode = GetStatusCode(result.IsSuccess, result.ErrorCode, (int)successStatusCode);

        return StatusCode(statusCode, result);
    }

    /// <summary>
    ///     Handles the BaseResult and returns the corresponding ActionResult with the appropriate HTTP status code.
    /// </summary>
    /// <param name="result">The BaseResult representing the outcome of the operation.</param>
    /// <returns>An ActionResult containing the BaseResult and corresponding HTTP status code.</returns>
    protected ActionResult<BaseResult> HandleBaseResult(BaseResult result)
    {
        var statusCode = GetStatusCode(result.IsSuccess, result.ErrorCode, StatusCodes.Status204NoContent);
        return StatusCode(statusCode, result);
    }

    private static int GetStatusCode(bool isSuccess, int? errorCode, int successStatusCode)
    {
        const int defaultCode = StatusCodes.Status400BadRequest;

        if (isSuccess) return successStatusCode;
        if (errorCode == null || !ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return defaultCode;
        return code;
    }
}