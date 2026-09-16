using System.Net;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Enums;
using UserService.Domain.Results;

namespace UserService.Api.Extensions;

public static class BaseResultExtensions
{
    private static readonly IReadOnlyDictionary<int, int> ErrorStatusCodeMap = new Dictionary<int, int>
    {
        // Reputation
        { (int)ErrorCodes.ReputationRuleNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.ReputationRulesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.ReputationRecordNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.ReputationRecordsNotFound, StatusCodes.Status404NotFound },

        // User
        { (int)ErrorCodes.UserNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.UserAlreadyExists, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.UserAlreadyHasThisRole, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.UsersNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.UsernameAlreadyTaken, StatusCodes.Status409Conflict },

        // Authorization
        { (int)ErrorCodes.InvalidCredentials, StatusCodes.Status401Unauthorized },
        { (int)ErrorCodes.InvalidToken, StatusCodes.Status400BadRequest },

        // Roles
        { (int)ErrorCodes.RoleAlreadyExists, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.RoleNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.RolesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.CannotDeleteDefaultRole, StatusCodes.Status403Forbidden },

        // Validity
        { (int)ErrorCodes.InvalidPagination, StatusCodes.Status400BadRequest },
        { (int)ErrorCodes.InvalidProperty, StatusCodes.Status400BadRequest }
    };

    /// <summary>
    ///     Converts a BaseResult of type T into the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    public static ActionResult<BaseResult<T>> ToActionResult<T>(
        this BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        if (result.IsSuccess) return new ObjectResult(result) { StatusCode = (int)successStatusCode };

        return new ObjectResult(result) { StatusCode = GetStatusCode(result.ErrorCode) };
    }

    /// <summary>
    ///     Converts a BaseResult into the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static ActionResult<BaseResult> ToActionResult(this BaseResult result)
    {
        if (result.IsSuccess) return new StatusCodeResult(StatusCodes.Status204NoContent);

        return new ObjectResult(result) { StatusCode = GetStatusCode(result.ErrorCode) };
    }

    private static int GetStatusCode(int? errorCode)
    {
        if (errorCode != null && ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return code;

        return StatusCodes.Status500InternalServerError;
    }
}