namespace UserService.Domain.Enum;

public enum ErrorCodes
{
    //User:21-30
    //Authorization:31-40
    //Roles: 41-50

    UserNotFound = 21,
    UserAlreadyExists = 22,
    UserAlreadyHasThisRole = 23,


    PasswordMismatch = 31,
    PasswordIsWrong = 32,
    EmailNotValid = 33,

    RoleAlreadyExists = 41,
    RoleNotFound = 42
}