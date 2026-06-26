namespace Learnup.Application.Authentication;

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAt);
