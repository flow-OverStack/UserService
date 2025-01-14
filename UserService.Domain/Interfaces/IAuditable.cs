namespace UserService.Domain.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
}