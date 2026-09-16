using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Application.Services.Provisioning;

public class UsernameGenerator(IBaseRepository<User> userRepository) : IUsernameGenerator
{
    public async Task<(string Username, bool IsTemporary)> ResolveUniqueUsernameAsync(
        string rawUsername, CancellationToken cancellationToken = default)
    {
        var sanitized = UsernameSanitizer.Sanitize(rawUsername);

        if (string.IsNullOrWhiteSpace(sanitized))
            return ("user", true);

        if (await userRepository.GetAll().AnyAsync(x => x.Username == sanitized, cancellationToken))
            return (sanitized, true);

        return (sanitized, false);
    }
}
