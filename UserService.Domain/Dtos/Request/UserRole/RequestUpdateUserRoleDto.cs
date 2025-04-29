namespace UserService.Domain.Dtos.Request.UserRole;

public class RequestUpdateUserRoleDto
{
    public long FromRoleId { get; set; }

    public long ToRoleId { get; set; }
}