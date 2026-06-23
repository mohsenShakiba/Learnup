namespace Learnup.API.Requests;

public sealed record UpdateUserBookCurrentPageRequest(string CurrentRef, float? Progress);
