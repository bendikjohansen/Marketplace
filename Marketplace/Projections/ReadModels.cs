namespace Marketplace.Projections;

public static class ReadModels
{
    public class ClassifiedAdDetails
    {
        public Guid ClassifiedAdId { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Description { get; set; }
        public Guid SellerId { get; set; }
        public string? SellersDisplayName { get; set; }
        public string[] PhotoUrls { get; set; } = { };
    }

    public class UserDetails
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
}
