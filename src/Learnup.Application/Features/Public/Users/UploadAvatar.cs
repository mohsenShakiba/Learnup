using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Users;

public sealed record UploadAvatar(Stream Content, string ContentType) : IRequest<UserProfileResponse?>;

internal sealed class UploadAvatarHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider,
    IFileService fileService)
    : IRequestHandler<UploadAvatar, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(
        UploadAvatar request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == identityProvider.UserId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var fileName = GetAvatarFileName(identityProvider.UserId, request.ContentType);
        var avatarFileId = await fileService.StoreAsync(new StoreFileRequest(
            request.Content,
            fileName,
            BucketNames.UserAvatarsBucket,
            request.ContentType), cancellationToken);

        user.UpdateAvatar(avatarFileId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(
            user.Id,
            user.DisplayName,
            user.AvatarUrl,
            user.CreatedAt,
            user.LastLogin,
            user.Status);
    }

    private static string GetAvatarFileName(int userId, string contentType)
    {
        var extension = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentOutOfRangeException(nameof(contentType), "Unsupported avatar content type.")
        };

        return $"{userId}/{Guid.NewGuid():N}{extension}";
    }
}
