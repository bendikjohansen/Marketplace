using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;

namespace Marketplace.Tests;

public class ClassifiedAdPublishSpec
{
    private readonly ClassifiedAd _classifiedAd;

    public ClassifiedAdPublishSpec()
    {
        _classifiedAd = new ClassifiedAd(new ClassifiedAdId(Guid.NewGuid()), new UserId(Guid.NewGuid()));
    }

    [Fact]
    public void Can_publish_a_valid_ad()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));
        _classifiedAd.AddPicture(new Uri("/ad/test", UriKind.Absolute), new PictureSize(1000, 1200));

        _classifiedAd.RequestToPublish();

        Assert.Equal(ClassifiedAdState.PendingReview, _classifiedAd.State);
    }

    [Fact]
    public void Cannot_publish_without_a_title()
    {
        _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<DomainExceptions.InvalidEntityState>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_without_a_text()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<DomainExceptions.InvalidEntityState>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_without_a_price()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));

        Assert.Throws<DomainExceptions.InvalidEntityState>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_with_zero_price()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(0, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<DomainExceptions.InvalidEntityState>(() => _classifiedAd.RequestToPublish());
    }
}
