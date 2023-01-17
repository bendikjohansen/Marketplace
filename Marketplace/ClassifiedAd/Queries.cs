using Marketplace.Domain.ClassifiedAd;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd;

public static class ReadModels
{
    public class ClassifiedAdDetails
    {
        public Guid ClassifiedAdId { get; init; }
        public string? Title { get; init; }
        public decimal? Price { get; init; }
        public string? CurrencyCode { get; init; }
        public string? Description { get; init; }
        public string? SellersDisplayName { get; init; }
        public string[] PhotoUrls { get; init; } = { };
    }

    public class ClassifiedAdListItem
    {
        public Guid ClassifiedAdId { get; init; }
        public string? Title { get; init; }
        public decimal? Price { get; init; }
        public string? CurrencyCode { get; init; }
        public string? PhotoUrl { get; init; }
    }
}

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
    private readonly IAsyncDocumentSession _session;

    public ClassifiedAdsQueryApi(IAsyncDocumentSession session) => _session = session;

    [HttpGet("/list")]
    public async Task<IActionResult> Get([FromQuery] QueryModels.GetPublishedClassifiedAds request)
    {
        var ads = await _session.Query(request);
        return Ok(ads);
    }

    [HttpGet("/my-ads")]
    public async Task<IActionResult> Get([FromQuery] QueryModels.GetOwnersClassifiedAds request)
    {
        var ads = await _session.Query(request);
        return Ok(ads);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] QueryModels.GetPublicClassifiedAd request)
    {
        var ads = await _session.Query(request);
        return Ok(ads);
    }
}

public static class Queries
{
    public static Task<List<ReadModels.ClassifiedAdListItem>> Query(this IAsyncDocumentSession session,
        QueryModels.GetPublishedClassifiedAds query)
        => session.Query<Domain.ClassifiedAd.ClassifiedAd>()
            .Where(x => x.State == ClassifiedAdState.Active)
            .Select(x => new ReadModels.ClassifiedAdListItem
            {
                ClassifiedAdId = x.Id.Value,
                CurrencyCode = x.Price.CurrencyCode,
                Title = x.Title.Value,
                Price = x.Price.Amount
            }).PagedList(query.Page, query.PageSize);

    public static Task<List<ReadModels.ClassifiedAdListItem>> Query(this IAsyncDocumentSession session,
        QueryModels.GetOwnersClassifiedAds query)
        => session.Query<Domain.ClassifiedAd.ClassifiedAd>()
            .Where(x => x.OwnerId == query.UserId)
            .Select(x => new ReadModels.ClassifiedAdListItem
            {
                ClassifiedAdId = x.Id.Value,
                CurrencyCode = x.Price.CurrencyCode,
                Title = x.Title.Value,
                Price = x.Price.Amount
            })
            .PagedList(query.Page, query.PageSize);

    public static Task<ReadModels.ClassifiedAdDetails> Query(this IAsyncDocumentSession session,
        QueryModels.GetPublicClassifiedAd query)
        => session.Query<Domain.ClassifiedAd.ClassifiedAd>()
            .Where(x => x.Id.Value == query.ClassifiedAdId)
            .Select(x => new ReadModels.ClassifiedAdDetails
            {
                ClassifiedAdId = x.Id.Value,
                CurrencyCode = x.Price.CurrencyCode,
                Title = x.Title.Value,
                Price = x.Price.Amount,
                Description = x.Text.Value,
                SellersDisplayName = RavenQuery.Load<Domain.UserProfile.UserProfile>($"UserProfile/{x.OwnerId.Value}")
                    .DisplayName.Value
            })
            .SingleAsync();

    private static Task<List<T>> PagedList<T>(this IRavenQueryable<T> query, int page, int pageSize) =>
        query.Skip(page * pageSize).Take(page).ToListAsync();
}
