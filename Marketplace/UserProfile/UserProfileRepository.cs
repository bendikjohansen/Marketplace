using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.UserProfile;

public class UserProfileRepository : RavenDbRepository<Domain.UserProfile.UserProfile, UserId>,
    IUserProfileRepository
{
    public UserProfileRepository(IAsyncDocumentSession session) : base(session, EntityId)
    {
    }

    private static string EntityId(UserId id) => $"UserProfile/{id}";
}
