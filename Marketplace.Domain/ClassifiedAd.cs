namespace Marketplace.Domain;

public class ClassifiedAd : Entity
{
    public ClassifiedAdId Id { get; }

    public UserId OwnerId { get; }
    public ClassifiedAdTitle? Title { get; private set; }
    public ClassifiedAdText? Text { get; private set; }
    public Price? Price { get; private set; }
    public ClassifiedAdState State { get; private set; }
    public UserId? ApprovedBy { get; private set; }

    public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
    {
        Id = id;
        OwnerId = ownerId;
        State = ClassifiedAdState.Inactive;
        EnsureValidState();

        Raise(new Events.ClassifiedAdCreated
        {
            Id = id,
            OwnerId = ownerId
        });
    }

    public void SetTitle(ClassifiedAdTitle title)
    {
        Title = title;
        EnsureValidState();

        Raise(new Events.ClassifiedAdTitleChanged
        {
            Id = Id,
            Title = title
        });
    }

    public void UpdateText(ClassifiedAdText text)
    {
        Text = text;
        EnsureValidState();

        Raise(new Events.ClassifiedAdTextUpdated
        {
            Id = Id,
            AdText = text.Value
        });
    }

    public void UpdatePrice(Price price)
    {
        Price = price;
        EnsureValidState();

        Raise(new Events.ClassifiedAdPriceUpdated
        {
            Id = Id,
            CurrencyCode = price.CurrencyCode,
            Price = price.Amount
        });
    }

    public void RequestToPublish()
    {
        State = ClassifiedAdState.PendingReview;
        EnsureValidState();

        Raise(new Events.ClassifiedAdSentForReview
        {
            Id = Id
        });
    }

    private void EnsureValidState()
    {
        var valid =
            State switch
            {
                ClassifiedAdState.PendingReview => Title != null && Text != null && Price?.Amount > 0,
                ClassifiedAdState.Active => Title != null && Text != null && ApprovedBy != null && Price?.Amount > 0,
                _ => true
            };
        if (!valid)
        {
            throw new InvalidEntityStateException(this, $"Post-checks failed in state {State}");
        }
    }
}

public enum ClassifiedAdState
{
    PendingReview,
    Active,
    Inactive,
    MarkedAsSold
}
