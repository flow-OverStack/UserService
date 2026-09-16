using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Results;

namespace UserService.Application.Extensions;

public static class CollectionResultExtensions
{
    extension<T>(CollectionResult<T>)
    {
        public static CollectionResult<T> UsersNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound)
        };

        public static CollectionResult<T> RolesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound)
        };

        public static CollectionResult<T> ReputationRulesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.ReputationRuleNotFound,
                (int)ErrorCodes.ReputationRuleNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.ReputationRulesNotFound,
                (int)ErrorCodes.ReputationRulesNotFound)
        };

        public static CollectionResult<T> ReputationRecordsNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.ReputationRecordNotFound,
                (int)ErrorCodes.ReputationRecordNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.ReputationRecordsNotFound,
                (int)ErrorCodes.ReputationRecordsNotFound)
        };
    }
}