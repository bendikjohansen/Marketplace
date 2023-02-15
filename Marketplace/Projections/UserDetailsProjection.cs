using Marketplace.Domain.UserProfile;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections;

public class UserDetailsProjection : RavenDbProjection<ReadModels.UserDetails>
{
    public UserDetailsProjection(Func<IAsyncDocumentSession> getSession) : base(getSession)
    {
    }

    public override Task Project(object @event) =>
        @event switch
        {
            Events.UserRegistered e =>
                Create(
                    () => Task.FromResult(
                        new ReadModels.UserDetails
                        {
                            UserId = e.UserId.ToString(),
                            DisplayName = e.DisplayName
                        }
                    )
                ),
            Events.UserDisplayNameUpdated e => UpdateOne(e.UserId, user => user.DisplayName = e.DisplayName),
            _ => Task.CompletedTask
        };
}
