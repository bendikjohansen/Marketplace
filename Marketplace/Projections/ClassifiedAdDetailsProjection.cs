using Marketplace.Domain.ClassifiedAd;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections;

public class ClassifiedAdDetailsProjection : RavenDbProjection<ReadModels.ClassifiedAdDetails>
{
    private readonly Func<Guid, Task<string>> _getUserDisplayName;

    public ClassifiedAdDetailsProjection(Func<IAsyncDocumentSession> getSession,
        Func<Guid, Task<string>> getUserDisplayName) : base(getSession)
    {
        _getUserDisplayName = getUserDisplayName;
    }

    public override Task Project(object @event)
        => @event switch
        {
            Events.ClassifiedAdCreated e =>
                Create(async () => new ReadModels.ClassifiedAdDetails
                {
                    ClassifiedAdId = e.Id,
                    SellerId = e.OwnerId,
                    SellersDisplayName = await _getUserDisplayName(e.OwnerId)
                }),
            Events.ClassifiedAdTitleChanged e => UpdateOne(e.Id, ad => { ad.Title = e.Title; }),
            Events.ClassifiedAdPriceUpdated e => UpdateOne(e.Id, ad =>
            {
                ad.Price = e.Price;
                ad.CurrencyCode = e.CurrencyCode;
            }),
            Events.ClassifiedAdTextUpdated e => UpdateOne(e.Id, ad => { ad.Description = e.AdText; }),
            Domain.UserProfile.Events.UserDisplayNameUpdated e => UpdateWhere(x => x.SellerId == e.UserId,
                x => x.SellersDisplayName = e.DisplayName),
            _ => Task.CompletedTask
        };
}
