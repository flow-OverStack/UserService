namespace UserService.Domain.Enum;

public enum ErrorCodes
{
    //User:21-30
    //Authorization:31-40
    //Roles: 41-50

    UserNotFound = 21,
    UserAlreadyExists = 22,
    UserAlreadyHasThisRole = 23,
    UsersNotFound = 24,

    PasswordIsWrong = 31,
    EmailNotValid = 32,

    RoleAlreadyExists = 41,
    RoleNotFound = 42,
    RolesNotFound = 43
}