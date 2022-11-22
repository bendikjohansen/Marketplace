namespace Marketplace.Domain.ClassifiedAd;

public static class Events
{
    public class ClassifiedAdCreated
    {
        public Guid Id { get; init; }
        public Guid OwnerId { get; init; }
    }

    public class ClassifiedAdTitleChanged
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
    }

    public class ClassifiedAdTextUpdated
    {
        public Guid Id { get; init; }
        public string AdText { get; init; }
    }

    public class ClassifiedAdPriceUpdated
    {
        public Guid Id { get; init; }
        public decimal Price { get; init; }
        public string CurrencyCode { get; init; }
    }

    public class ClassifiedAdSentForReview
    {
        public Guid Id { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
    }

    public class PictureAddedToAClassifiedAd
    {
        public Guid ClassifiedAdId { get; init; }
        public Guid PictureId { get; init; }
        public string Url { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int Order { get; init; }
    }

    public class ClassifiedAdPictureResized
    {
        public Guid PictureId { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }
    }

    public class ClassifiedAdPublished
    {
        public Guid Id { get; init; }
        public Guid ModeratorId { get; init; }
    }
}
