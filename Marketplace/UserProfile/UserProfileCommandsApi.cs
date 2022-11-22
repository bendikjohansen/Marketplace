using Microsoft.AspNetCore.Mvc;

namespace Marketplace.UserProfile;

[Route("userprofile")]
public class UserProfileCommandsApi : ControllerBase
{
    private readonly UserProfileApplicationService _service;

    public UserProfileCommandsApi(UserProfileApplicationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Contracts.V1.RegisterUser request)
    {
        await _service.Handle(request);
        return Ok();
    }

    [HttpPut("displayName")]
    public async Task<IActionResult> Put(Contracts.V1.UpdateUserDisplayName request)
    {
        await _service.Handle(request);
        return Ok();
    }

    [HttpPut("fullname")]
    public async Task<IActionResult> Put(Contracts.V1.UpdateUserFullName request)
    {
        await _service.Handle(request);
        return Ok();
    }

    [HttpPut("photo")]
    public async Task<IActionResult> Put(Contracts.V1.UpdateUserProfilePhoto request)
    {
        await _service.Handle(request);
        return Ok();
    }
}
