namespace UserService.Domain.Enums;

public enum ErrorCodes
{
    //Reputation: 11-20
    //User: 21-30
    //Authorization: 31-40
    //Roles: 41-50

    DailyReputationLimitExceeded = 11,
    ReputationMinimumReached = 12,
    CannotIncreaseOrDecreaseNegativeReputation = 13,

    UserNotFound = 21,
    UserAlreadyExists = 22,
    UserAlreadyHasThisRole = 23,
    UsersNotFound = 24,

    PasswordIsWrong = 31,
    EmailNotValid = 32,
    InvalidToken = 33,

    RoleAlreadyExists = 41,
    RoleNotFound = 42,
    RolesNotFound = 43
}