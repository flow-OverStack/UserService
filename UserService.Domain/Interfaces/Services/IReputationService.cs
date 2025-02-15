using UserService.Domain.Dto.User;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IReputationService
{
    Task<BaseResult<ReputationDto>> IncreaseReputation(ReputationIncreaseDto dto);

    Task<BaseResult<ReputationDto>> DecreaseReputation(ReputationDecreaseDto dto);
}