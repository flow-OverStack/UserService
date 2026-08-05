using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Application.Services.Provisioning;

/// <summary>
///     Creates the local DB record for a user known to the identity server but not yet
///     provisioned locally - on first login (via <see cref="Domain.Interfaces.Service.IUserSyncService" />)
///     or on an explicit init call after registration.
/// </summary>
public interface IUserProvisioningService
{
    Task<BaseResult<UserDto>> InitAsync(InitUserDto dto, CancellationToken cancellationToken = default);
}
