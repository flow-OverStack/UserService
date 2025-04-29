namespace UserService.Domain.Interfaces.Entity;

public interface IEntityId<T> where T : struct
{
    public T Id { get; set; }
}