namespace UserService.Domain.Enum;

public enum ErrorCodes
{
    //Report:0-9
    //Internals: 10-20
    //User:21-30
    //Authorization:31-40
    //Roles: 41-50
    InternalServerError = 10,
    IdentityServerError = 11,

    ReportsNotFound = 0,
    ReportNotFound = 1,

    UserNotFound = 21,
    UserAlreadyExists = 22,
    UserAlreadyHasThisRole = 23,


    PasswordMismatch = 31,
    PasswordIsWrong = 32,

    RoleAlreadyExists = 41,
    RoleNotFound = 42
}