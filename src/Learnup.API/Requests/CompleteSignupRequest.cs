namespace Learnup.API.Requests;

public sealed record CompleteSignupRequest(
    string MobileNumber,
    string Code,
    string DisplayName,
    string? AvatarUrl);
