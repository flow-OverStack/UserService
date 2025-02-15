using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IReputationResetService
{
    Task<BaseResult> ResetReputation();
}