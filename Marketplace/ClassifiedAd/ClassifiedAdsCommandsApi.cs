using Microsoft.AspNetCore.Mvc;
using static Marketplace.ClassifiedAd.Contracts;

namespace Marketplace.ClassifiedAd;

[Route("/ad")]
public class ClassifiedAdsCommandsApi : ControllerBase
{
    private readonly ClassifiedAdsApplicationService _applicationService;

    public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService applicationService) => _applicationService = applicationService;

    [HttpPost]
    public async Task<IActionResult> Post(V1.Create request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("name")]
    [HttpPut]
    public async Task<IActionResult> Put(V1.SetTitle request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("text")]
    [HttpPut]
    public async Task<IActionResult> Put(V1.UpdateText request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("price")]
    [HttpPut]
    public async Task<IActionResult> Put(V1.UpdatePrice request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("picture")]
    [HttpPost]
    public async Task<IActionResult> AddPicture(V1.AddPicture request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("publish")]
    [HttpPut]
    public async Task<IActionResult> Put(V1.RequestToPublish request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }

    [Route("approve")]
    [HttpPut]
    public async Task<IActionResult> Put(V1.ApprovePublish request)
    {
        await _applicationService.Handle(request);
        return Ok();
    }
}
