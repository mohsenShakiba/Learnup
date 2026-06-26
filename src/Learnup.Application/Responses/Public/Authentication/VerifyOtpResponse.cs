namespace Learnup.Application.Responses.Public.Authentication;

public sealed record VerifyOtpResponse(string AccessToken, DateTime ExpiresAt);
