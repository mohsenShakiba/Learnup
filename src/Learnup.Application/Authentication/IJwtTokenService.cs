using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(User user);
}
