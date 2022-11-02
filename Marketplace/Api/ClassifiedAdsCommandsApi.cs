using Marketplace.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api;

[Route("/ad")]
public class ClassifiedAdsCommandsApi : Controller
{
    private readonly ClassifiedAdsApplicationService _service;

    public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Post(ClassifiedAds.V1.Create request)
    {
        _service.Handle(request);

        return Ok();
    }
}
