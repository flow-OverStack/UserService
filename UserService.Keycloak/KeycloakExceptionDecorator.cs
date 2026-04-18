using UserService.Application.Exceptions.IdentityServer;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Identity;

namespace UserService.Keycloak;

public class KeycloakExceptionDecorator(IIdentityServer inner) : IIdentityServer
{
    public Task<IdentityUserDto> RegisterUserAsync(IdentityRegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        return ExecuteSafeAsync(() => inner.RegisterUserAsync(dto, cancellationToken));
    }

    public Task<IdentityUserDto?> FindUserAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return ExecuteSafeAsync(() => inner.FindUserAsync(identifier, cancellationToken));
    }

    public Task<TokenDto> LoginUserAsync(IdentityLoginUserDto dto, CancellationToken cancellationToken = default)
    {
        return ExecuteSafeAsync(() => inner.LoginUserAsync(dto, cancellationToken));
    }

    public Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        return ExecuteSafeAsync(() => inner.RefreshTokenAsync(dto, cancellationToken));
    }

    public Task UpdateUserAsync(IdentityUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        return ExecuteSafeAsync(() => inner.UpdateUserAsync(dto, cancellationToken));
    }

    public Task DeleteUserAsync(IdentityUserIdDto dto)
    {
        return ExecuteSafeAsync(() => inner.DeleteUserAsync(dto));
    }

    private static async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException("Keycloak", e.Message, e);
        }
    }

    private static async Task ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException("Keycloak", e.Message, e);
        }
    }
}