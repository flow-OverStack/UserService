namespace UserService.Domain.Dto.Requests.UserRole;

public class RequestUpdateUserRoleDto
{
    public long FromRoleId { get; set; }

    public long ToRoleId { get; set; }
}