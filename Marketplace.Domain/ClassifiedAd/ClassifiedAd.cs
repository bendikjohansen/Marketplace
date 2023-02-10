using Marketplace.Domain.Shared;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
{
    public UserId OwnerId { get; private set; }
    public ClassifiedAdTitle? Title { get; private set; }
    public ClassifiedAdText? Text { get; private set; }
    public Price? Price { get; private set; }
    public ClassifiedAdState State { get; private set; }
    public UserId? ApprovedBy { get; private set; }
    public List<Picture> Pictures { get; }

    public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
    {
        Pictures = new List<Picture>();
        Apply(new Events.ClassifiedAdCreated
        {
            Id = id,
            OwnerId = ownerId
        });
    }

    public void SetTitle(ClassifiedAdTitle title) =>
        Apply(new Events.ClassifiedAdTitleChanged
        {
            Id = Id,
            Title = title
        });

    public void UpdateText(ClassifiedAdText text) =>
        Apply(new Events.ClassifiedAdTextUpdated
        {
            Id = Id,
            AdText = text
        });

    public void UpdatePrice(Price price) =>
        Apply(new Events.ClassifiedAdPriceUpdated
        {
            Id = Id,
            CurrencyCode = price.CurrencyCode,
            Price = price.Amount
        });

    public void AddPicture(Uri pictureUri, PictureSize size) =>
        Apply(new Events.PictureAddedToAClassifiedAd
        {
            ClassifiedAdId = Id,
            PictureId = Guid.NewGuid(),
            Width = size.Width,
            Height = size.Height,
            Url = pictureUri.ToString(),
            Order = Pictures.Select(pic => pic.Order).DefaultIfEmpty(0).Max() + 1
        });

    public void ResizePicture(PictureId pictureId, PictureSize newSize)
    {
        var picture = FindPicture(pictureId);
        if (picture == null)
        {
            throw new InvalidOperationException("Cannot resize a picture that you do not have");
        }

        picture.Resize(newSize);
    }

    public void RequestToPublish() => Apply(new Events.ClassifiedAdSentForReview { Id = Id });

    public void Publish(UserId moderatorId) =>
        Apply(new Events.ClassifiedAdPublished { Id = Id, ModeratorId = moderatorId });

    protected override void When(object @event)
    {
        switch (@event)
        {
            case Events.ClassifiedAdCreated e:
                Id = new ClassifiedAdId(e.Id);
                OwnerId = new UserId(e.OwnerId);
                State = ClassifiedAdState.Inactive;
                break;
            case Events.ClassifiedAdTitleChanged e:
                Title = new ClassifiedAdTitle(e.Title);
                break;
            case Events.ClassifiedAdTextUpdated e:
                Text = new ClassifiedAdText(e.AdText);
                break;
            case Events.ClassifiedAdPriceUpdated e:
                Price = new Price(e.Price, e.CurrencyCode);
                break;
            case Events.ClassifiedAdSentForReview e:
                State = ClassifiedAdState.PendingReview;
                break;
            case Events.PictureAddedToAClassifiedAd e:
                var picture = new Picture(Apply);
                ApplyToEntity(picture, e);
                Pictures.Add(picture);
                break;
            case Events.ClassifiedAdPublished e:
                State = ClassifiedAdState.Active;
                ApprovedBy = new UserId(e.ModeratorId);
                break;
        }
    }

    protected override void EnsureValidState()
    {
        var valid =
            State switch
            {
                ClassifiedAdState.PendingReview => Title != null
                                                   && Text != null
                                                   && Price?.Amount > 0
                                                   && FirstPicture.HasCorrectSize(),
                ClassifiedAdState.Active => Title != null
                                            && Text != null
                                            && ApprovedBy != null
                                            && Price?.Amount > 0
                                            && FirstPicture.HasCorrectSize(),
                _ => true
            };
        if (!valid)
        {
            throw new DomainExceptions.InvalidEntityState(this, $"Post-checks failed in state {State}");
        }
    }

    public ClassifiedAd()
    {
        Pictures = new List<Picture>();
    }

    private Picture? FirstPicture => Pictures.MinBy(x => x.Order);
    private Picture? FindPicture(PictureId id) => Pictures.FirstOrDefault(x => x.Id == id);
}

public enum ClassifiedAdState
{
    PendingReview,
    Active,
    Inactive,
    MarkedAsSold
}
