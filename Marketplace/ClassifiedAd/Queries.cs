using Marketplace.Projections;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd;

public static class QueryModels
{
    public record GetPublicClassifiedAd(Guid ClassifiedAdId);
}

[Route("/ad")]
[ApiController]
public class ClassifiedAdsQueryApi : ControllerBase
{
    private readonly IAsyncDocumentSession _session;

    public ClassifiedAdsQueryApi(IAsyncDocumentSession session) => _session = session;

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] QueryModels.GetPublicClassifiedAd request)
        => Ok(await _session.Query(request));
}

public static class Queries
{
    public static Task<ReadModels.ClassifiedAdDetails> Query(this IAsyncDocumentSession session,
        QueryModels.GetPublicClassifiedAd query)
        => session.LoadAsync<ReadModels.ClassifiedAdDetails>(query.ClassifiedAdId.ToString());

}
