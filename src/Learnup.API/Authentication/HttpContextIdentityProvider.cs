using System.Security.Claims;
using Learnup.Application.Authentication;

namespace Learnup.API.Authentication;

public sealed class HttpContextIdentityProvider(IHttpContextAccessor httpContextAccessor) : IIdentityProvider
{
    public int UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Authenticated user id was not found.");
        }
    }
}
