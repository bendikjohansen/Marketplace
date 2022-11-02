using Marketplace.Domain;
using Marketplace.Framework;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Api;

public class ClassifiedAdsApplicationService
{
    private readonly IEntityStore _store;
    private readonly ICurrencyLookup _currencyLookup;

    public Task Handle(object command) => command switch
    {
        V1.Create cmd => HandleCreate(cmd),
        V1.SetTitle cmd => HandleUpdate(cmd.Id,
            ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))),
        V1.UpdateText cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))),
        V1.UpdatePrice cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdatePrice(Price.FromDecimal(cmd.Amount, cmd.CurrencyCode, _currencyLookup))),
        V1.RequestToPublish cmd => HandleUpdate(cmd.Id, ad => ad.RequestToPublish()),
        _ => Task.CompletedTask
    };

    private async Task HandleCreate(V1.Create cmd)
    {
        if (await _store.Exists<ClassifiedAd>(cmd.Id.ToString()))
        {
            throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");
        }

        var classifiedAd = new ClassifiedAd(
            new ClassifiedAdId(cmd.Id),
            new UserId(cmd.OwnerId));

        await _store.Save(classifiedAd);
    }

    private async Task HandleUpdate(Guid id, Action<ClassifiedAd> operation)
    {
        var classifiedAd = await _store.Load<ClassifiedAd>(id.ToString());
        if (classifiedAd == null)
        {
            throw new InvalidOperationException($"Entity with id {id} cannot be found");
        }

        operation(classifiedAd);
        await _store.Save(classifiedAd);
    }
}
