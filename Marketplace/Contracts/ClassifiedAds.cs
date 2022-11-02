namespace Marketplace.Contracts;

public static class ClassifiedAds
{
    public static class V1
    {
        public class Create
        {
            public Guid Id { get; init; }
            public Guid OwnerId { get; init; }
        }
    }
}
