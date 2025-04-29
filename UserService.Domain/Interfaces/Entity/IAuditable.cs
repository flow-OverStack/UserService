namespace UserService.Domain.Interfaces.Entity;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
}