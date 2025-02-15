using Microsoft.EntityFrameworkCore;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class ReputationService(IBaseRepository<User> userRepository) : IReputationService, IReputationResetService
{
    public async Task<BaseResult> ResetReputation()
    {
        try
        {
            await userRepository.GetAll().ForEachAsync(x => x.ReputationEarnedToday = User.MinReputation);

            return BaseResult.Success();
        }
        catch (Exception e)
        {
            return BaseResult.Failure(e.Message);
        }
    }

    public async Task<BaseResult<ReputationDto>> IncreaseReputation(ReputationIncreaseDto dto)
    {
        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToIncrease = CalculateReputationToIncrease(user.ReputationEarnedToday, dto.ReputationToIncrease);
        if (reputationToIncrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.DailyReputationLimitExceeded,
                (int)ErrorCodes.DailyReputationLimitExceeded);

        user.Reputation += reputationToIncrease;
        user.ReputationEarnedToday += reputationToIncrease;

        await userRepository.SaveChangesAsync();

        var remainingDailyLimit = User.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = dto.UserId,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    public async Task<BaseResult<ReputationDto>> DecreaseReputation(ReputationDecreaseDto dto)
    {
        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToDecrease = CalculateReputationToDecrease(user.Reputation, dto.ReputationToDecrease);
        if (reputationToDecrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.ReputationMinimumReached,
                (int)ErrorCodes.ReputationMinimumReached);

        user.Reputation -= reputationToDecrease;

        await userRepository.SaveChangesAsync();

        var remainingDailyLimit = User.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = user.Id,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    private static int CalculateReputationToIncrease(int reputationEarnedToday, int reputationToIncrease)
    {
        var remainingReputation = User.MaxDailyReputation - reputationEarnedToday;
        if (remainingReputation > 0)
            return Math.Min(remainingReputation, reputationToIncrease);

        return 0;
    }

    private static int CalculateReputationToDecrease(int currentReputation, int reputationToDecrease)
    {
        var newReputation = currentReputation - reputationToDecrease;
        if (newReputation < User.MinReputation)
            return currentReputation - User.MinReputation;

        return reputationToDecrease;
    }
}