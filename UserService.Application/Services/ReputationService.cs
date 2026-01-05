using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class ReputationService(IUnitOfWork unitOfWork) : IReputationService
{
    public async Task<BaseResult> ApplyReputationEventAsync(ReputationEventDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.EventType is BaseEventType.EntityDeleted)
            return await RevokeReputationForEntityAsync(dto.EntityId, dto.EntityType, cancellationToken);

        var user = await unitOfWork.Users.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);
        if (user == null)
            return BaseResult.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var rule = await unitOfWork.ReputationRules.GetAll()
            .FirstOrDefaultAsync(x => x.EventType == dto.EventType.ToString(), cancellationToken);
        if (rule == null)
            return BaseResult.Failure(ErrorMessage.ReputationRuleNotFound, (int)ErrorCodes.ReputationRuleNotFound);

        return await ApplyReputationRuleAsync(rule, user.Id, dto.EntityId, cancellationToken);
    }

    private async Task<BaseResult> ApplyReputationRuleAsync(ReputationRule rule, long userId,
        long entityId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var oldRecords = unitOfWork.ReputationRecords.GetAll()
                .Include(x => x.ReputationRule)
                .Where(x => x.UserId == userId && x.EntityId == entityId && x.ReputationRule.Group != null &&
                            x.ReputationRule.Group == rule.Group);

            await oldRecords.ExecuteUpdateAsync(x => x.SetProperty(p => p.Enabled, p => false), cancellationToken);

            var newRecord = new ReputationRecord
            {
                UserId = userId,
                ReputationRuleId = rule.Id,
                EntityId = entityId
            };

            await unitOfWork.ReputationRecords.CreateAsync(newRecord, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return BaseResult.Success();
    }

    private async Task<BaseResult> RevokeReputationForEntityAsync(long entityId, EntityType entityType,
        CancellationToken cancellationToken = default)
    {
        var records = unitOfWork.ReputationRecords.GetAll()
            .Include(x => x.ReputationRule)
            .Where(x => x.EntityId == entityId && x.ReputationRule.EntityType == entityType.ToString());

        await records.ExecuteUpdateAsync(x => x.SetProperty(p => p.Enabled, p => false), cancellationToken);
        return BaseResult.Success();
    }
}