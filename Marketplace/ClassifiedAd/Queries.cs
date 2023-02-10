using Marketplace.Projections;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.ClassifiedAd;

public static class QueryModels
{
    public record GetPublishedClassifiedAds(int Page, int PageSize);

    public record GetOwnersClassifiedAds(Guid UserId, int Page, int PageSize);

    public record GetPublicClassifiedAd(Guid ClassifiedAdId);
}

[Route("/ad")]
[ApiController]
public class ClassifiedAdsQueryApi : ControllerBase
{
    private readonly IList<ReadModels.ClassifiedAdDetails> _items;

    public ClassifiedAdsQueryApi(IList<ReadModels.ClassifiedAdDetails> items) => _items = items;

    [HttpGet]
    public ActionResult Get([FromQuery] QueryModels.GetPublicClassifiedAd request)
        => Ok(_items.Query(request));
}

public static class Queries
{
    public static ReadModels.ClassifiedAdDetails? Query(this IEnumerable<ReadModels.ClassifiedAdDetails> items,
        QueryModels.GetPublicClassifiedAd query)
        => items.FirstOrDefault(x => x.ClassifiedAdId == query.ClassifiedAdId);

}
