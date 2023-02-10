using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public interface IClassifiedAdRepository : IAggregateStore
{
    Task<ClassifiedAd?> Load(ClassifiedAdId id);
    Task Save(ClassifiedAd entity);
    Task<bool> Exists(ClassifiedAdId id);
}
