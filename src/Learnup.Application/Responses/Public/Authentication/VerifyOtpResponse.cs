namespace Learnup.Application.Responses.Public.Authentication;

public sealed record VerifyOtpResponse(string? AccessToken, DateTime? ExpiresAt, bool RequiresSignup)
{
    public static VerifyOtpResponse SignedIn(string accessToken, DateTime expiresAt)
    {
        return new VerifyOtpResponse(accessToken, expiresAt, false);
    }

    public static VerifyOtpResponse SignupRequired()
    {
        return new VerifyOtpResponse(null, null, true);
    }
}
