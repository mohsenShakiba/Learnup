namespace Learnup.API.Requests;

public sealed record VerifyOtpRequest(string MobileNumber, string Code);
