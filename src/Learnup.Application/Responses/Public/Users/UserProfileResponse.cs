using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Responses.Public.Users;

public sealed record UserProfileResponse(
    int Id,
    string DisplayName,
    string? AvatarUrl,
    DateTime CreatedAt,
    DateTime? LastLogin,
    UserStatus Status);
