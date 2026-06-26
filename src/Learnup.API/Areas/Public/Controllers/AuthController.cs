using Learnup.API.Requests;
using Learnup.Application.Features.Public.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

[AllowAnonymous]
public class AuthController(IMediator mediator) : BasePublicController
{
    [HttpPost("send-otp", Name = "SendOtp")]
    public async Task<ActionResult<SendOtpResponse>> SendOtp(
        SendOtpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new SendOtp(request.MobileNumber), cancellationToken);
        return Ok(response);
    }

    [HttpPost("verify-otp", Name = "VerifyOtp")]
    public async Task<ActionResult<VerifyOtpResponse>> VerifyOtp(
        VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new VerifyOtp(request.MobileNumber, request.Code), cancellationToken);
        return response is null
            ? Unauthorized()
            : Ok(response);
    }
}
