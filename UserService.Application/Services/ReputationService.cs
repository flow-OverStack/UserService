using System.Linq.Expressions;
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
    public Task<BaseResult> ApplyReputationEventAsync(ReputationEventDto dto,
        CancellationToken cancellationToken = default)
    {
        return dto.EventType switch
        {
            BaseEventType.EntityDeleted => RevokeReputationForEntityAsync(dto.EntityId, dto.EntityType.ToString(),
                cancellationToken),
            BaseEventType.EntityAcceptanceRevoked => RevokeAcceptanceAsync(dto.EntityId,
                dto.EntityType.ToString(), cancellationToken),
            BaseEventType.EntityVoteRemoved => RemoveVoteAsync(dto.AuthorId, dto.InitiatorId, dto.EntityId,
                dto.EntityType.ToString(),
                cancellationToken),

            _ => ApplyReputationEventAsync(dto.AuthorId, dto.InitiatorId, dto.EntityId, dto.EntityType.ToString(),
                dto.EventType.ToString(), cancellationToken)
        };
    }

    private async Task<BaseResult> ApplyReputationEventAsync(long authorId, long initiatorId, long entityId,
        string entityType, string eventType, CancellationToken cancellationToken = default)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Finding any rule for the current entity
            var rules = await unitOfWork.ReputationRules.GetAll()
                .Where(x => x.EventType == eventType && x.EntityType == entityType)
                .ToArrayAsync(cancellationToken);

            if (rules.Length == 0)
                return BaseResult.Failure(ErrorMessage.ReputationRulesNotFound,
                    (int)ErrorCodes.ReputationRulesNotFound);

            // Domain rule: for a given EventType + EntityType, Group is either NULL or identical across all ReputationRule records.
            var group = rules.Select(x => x.Group).Distinct().Single();

            // Disabling all records for both the initiator and the author if the records are in the same group as the rule 
            await DisableReputationRecordsAsync(x =>
                ((x.UserId == authorId && x.ReputationRule.ReputationTarget == ReputationTarget.Author) ||
                 (x.UserId == initiatorId && x.ReputationRule.ReputationTarget == ReputationTarget.Initiator))
                && x.ReputationRule.EntityType == entityType
                && x.EntityId == entityId
                && x.ReputationRule.Group != null
                && x.ReputationRule.Group == group, cancellationToken);

            // Applying a reputation rule for the author
            var authorRule = rules.FirstOrDefault(x => x.ReputationTarget == ReputationTarget.Author);
            var authorResult = await ApplyReputationRuleAsync(authorRule, authorId, entityId, cancellationToken);
            if (!authorResult.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                return authorResult;
            }

            // There can be no rules for initiator
            var initiatorRule = rules.FirstOrDefault(x => x.ReputationTarget == ReputationTarget.Initiator);
            if (initiatorRule != null)
                await ApplyReputationRuleAsync(initiatorRule, initiatorId, entityId, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return BaseResult.Success();
    }

    private async Task<BaseResult> ApplyReputationRuleAsync(ReputationRule? rule, long userId, long entityId,
        CancellationToken cancellationToken = default)
    {
        if (rule == null)
            return BaseResult.Failure(ErrorMessage.ReputationRuleNotFound, (int)ErrorCodes.ReputationRuleNotFound);

        var user = await unitOfWork.Users.GetAll().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user == null)
            return BaseResult.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var newRecord = new ReputationRecord
        {
            UserId = userId,
            ReputationRuleId = rule.Id,
            EntityId = entityId
        };

        await unitOfWork.ReputationRecords.CreateAsync(newRecord, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    private Task<BaseResult> RevokeReputationForEntityAsync(long entityId, string entityType,
        CancellationToken cancellationToken = default)
    {
        return DisableReputationRecordsAsync(
            x => x.EntityId == entityId && x.ReputationRule.EntityType == entityType, cancellationToken);
    }


    private Task<BaseResult> RemoveVoteAsync(long authorId, long initiatorId, long entityId,
        string entityType, CancellationToken cancellationToken = default)
    {
        return DisableReputationRecordsAsync(x =>
                ((x.UserId == authorId && x.ReputationRule.ReputationTarget == ReputationTarget.Author) ||
                 (x.UserId == initiatorId && x.ReputationRule.ReputationTarget == ReputationTarget.Initiator))
                && x.ReputationRule.EntityType == entityType
                && x.EntityId == entityId
                && (x.ReputationRule.EventType ==
                    nameof(BaseEventType.EntityUpvoted) ||
                    x.ReputationRule.EventType ==
                    nameof(BaseEventType.EntityDownvoted)),
            cancellationToken);
    }

    private Task<BaseResult> RevokeAcceptanceAsync(long entityId, string entityType,
        CancellationToken cancellationToken = default)
    {
        return DisableReputationRecordsAsync(x => x.ReputationRule.EntityType == entityType
                                                  && x.EntityId == entityId
                                                  && x.ReputationRule.EventType == nameof(BaseEventType.EntityAccepted),
            cancellationToken);
    }


    private async Task<BaseResult> DisableReputationRecordsAsync(Expression<Func<ReputationRecord, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var record = unitOfWork.ReputationRecords.GetAll()
            .Include(x => x.ReputationRule)
            .Where(predicate);

        await DisableReputationRecordsAsync(record, cancellationToken);
        return BaseResult.Success();
    }

    private static Task<int> DisableReputationRecordsAsync(IQueryable<ReputationRecord> records,
        CancellationToken cancellationToken = default)
    {
        return records.ExecuteUpdateAsync(x => x.SetProperty(p => p.Enabled, p => false), cancellationToken);
    }
}