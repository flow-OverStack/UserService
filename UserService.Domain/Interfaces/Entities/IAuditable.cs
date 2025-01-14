namespace UserService.Domain.Interfaces.Entities;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
}