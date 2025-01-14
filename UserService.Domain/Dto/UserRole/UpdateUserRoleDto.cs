namespace UserService.Domain.Dto.UserRole;

public class UpdateUserRoleDto
{
    public string Username { get; set; }

    public long FromRoleId { get; set; }

    public long ToRoleId { get; set; }
}