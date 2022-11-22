using Marketplace.Domain.ClassifiedAd;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdRepository : RavenDbRepository<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>,
    IClassifiedAdRepository
{
    public ClassifiedAdRepository(IAsyncDocumentSession session) : base(session, EntityId)
    {
    }

    private static string EntityId(ClassifiedAdId id) => $"ClassifiedAd/{id}";
}
