using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Api.Controllers;

/// <summary>
///     User activity controller
/// </summary>
/// <param name="activityService"></param>
[Authorize]
public class UserActivityController(IUserActivityService activityService) : BaseController
{
    /// <summary>
    ///     Registers user's last activity time.
    ///     The frontend is responsible for controlling the intervals between endpoint calls.
    ///     The accuracy of user activity tracking depends on the call frequency.
    ///     For example, if called weekly, the accuracy will be up to one week;
    ///     if called daily, the accuracy will be up to one day, and so on.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     POST heartbeat
    /// </remarks>
    [HttpPost("heartbeat")]
    public async Task<ActionResult<BaseResult>> RegisterHeartbeat(
        CancellationToken cancellationToken)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized("Invalid user identifier.");

        var result = await activityService.RegisterHeartbeatAsync(userId, cancellationToken);

        return HandleBaseResult(result);
    }
}