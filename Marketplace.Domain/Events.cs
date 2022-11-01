namespace Marketplace.Domain;

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
    }
}
