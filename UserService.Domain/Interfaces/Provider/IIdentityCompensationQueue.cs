using UserService.Domain.Dtos.Identity;

namespace UserService.Domain.Interfaces.Provider;

public interface IIdentityCompensationQueue
{
    void EnqueueIdentityUserUpdate(IdentityUpdateUserDto dto);
    void EnqueueIdentityUserDeletion(string identityId);
}
