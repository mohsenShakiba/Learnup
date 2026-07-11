namespace Learnup.Application.Responses.Public.Ai;

public sealed record TokenUsageResponse(
    long UsageLimit,
    long CurrentUsage);
