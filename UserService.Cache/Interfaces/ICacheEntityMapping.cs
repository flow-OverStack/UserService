namespace UserService.Cache.Interfaces;

public interface ICacheEntityMapping<in TEntity, TEntityId>
{
    TEntityId GetId(TEntity entity);

    string GetKey(TEntityId id);
    string GetValue(TEntity entity);

    TEntityId ParseIdFromKey(string key);
    TEntityId ParseIdFromValue(string value);
}