using Marketplace.Domain;

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
        _classifiedAd.UpdateText(new ClassifiedAdText("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));

        _classifiedAd.RequestToPublish();

        Assert.Equal(ClassifiedAdState.PendingReview, _classifiedAd.State);
    }

    [Fact]
    public void Cannot_publish_without_a_title()
    {
        _classifiedAd.UpdateText(new ClassifiedAdText("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_without_a_text()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(10, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_without_a_price()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdateText(new ClassifiedAdText("Please buy my stuff"));

        Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
    }

    [Fact]
    public void Cannot_publish_with_zero_price()
    {
        _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
        _classifiedAd.UpdateText(new ClassifiedAdText("Please buy my stuff"));
        _classifiedAd.UpdatePrice(Price.FromDecimal(0, "EUR", new FakeCurrencyLookup()));

        Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
    }
}
