using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using static Marketplace.ClassifiedAd.Contracts;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdsApplicationService : IApplicationService
{
    private readonly IAggregateStore _store;
    private readonly ICurrencyLookup _currencyLookup;

    public ClassifiedAdsApplicationService(IAggregateStore store, ICurrencyLookup currencyLookup)
    {
        _store = store;
        _currencyLookup = currencyLookup;
    }

    public Task Handle(object command) => command switch
    {
        V1.Create cmd => HandleCreate(cmd),
        V1.SetTitle cmd => HandleUpdate(cmd.Id,
            ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))),
        V1.UpdateText cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))),
        V1.UpdatePrice cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdatePrice(Price.FromDecimal(cmd.Amount, cmd.CurrencyCode, _currencyLookup))),
        V1.AddPicture cmd => HandleUpdate(cmd.Id, ad => ad.AddPicture(cmd.Url, new PictureSize(cmd.Width, cmd.Height))),
        V1.RequestToPublish cmd => HandleUpdate(cmd.Id, ad => ad.RequestToPublish()),
        V1.ApprovePublish cmd => HandleUpdate(cmd.Id, ad => ad.Publish(new UserId(cmd.UserId))),
        _ => Task.CompletedTask
    };

    private async Task HandleCreate(V1.Create cmd)
    {
        if (await _store.Exists<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(new ClassifiedAdId(cmd.Id)))
        {
            throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");
        }

        var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
            new ClassifiedAdId(cmd.Id),
            new UserId(cmd.OwnerId));

        await _store.Save<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(classifiedAd);
    }

    private Task HandleUpdate(Guid id, Action<Domain.ClassifiedAd.ClassifiedAd> update)
        => this.HandleUpdate(_store, new ClassifiedAdId(id), update);
}
