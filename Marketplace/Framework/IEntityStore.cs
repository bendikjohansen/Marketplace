using Marketplace.Domain;

namespace Marketplace.Framework;

public interface IEntityStore
{
    Task<bool> Exists<T>(string id);
    Task<T> Save<T>(T entity) where T : Entity;
    Task<T> Load<T>(string id) where T : Entity;
}
