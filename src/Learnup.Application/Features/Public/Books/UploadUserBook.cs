using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Features.Public.Books;

public sealed record UploadUserBook(
    Stream Content,
    string ContentType,
    long Length,
    string Title,
    Stream? Cover,
    string? CoverContentType) : IRequest;

internal sealed class UploadUserBookHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider,
    IFileService fileService)
    : IRequestHandler<UploadUserBook>
{
    public async Task<Unit> Handle(UploadUserBook request, CancellationToken cancellationToken)
    {
        if (request.Length <= 0)
        {
            throw new ArgumentException("Book file cannot be empty.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Book title is required.", nameof(request));
        }

        var fileName = GetEbookFileName(GetRandomName(), request.ContentType);

        var bookFile = await fileService.StoreAsync(new StoreFileRequest(
            request.Content,
            fileName,
            BucketNames.BooksBucket,
            request.ContentType), cancellationToken);

        StoredFile? storedCover = null;
        if (request.Cover is not null && !string.IsNullOrWhiteSpace(request.CoverContentType))
        {
            var coverName = GetCoverFileName(GetRandomName(), request.CoverContentType);
            storedCover = await fileService.StoreAsync(new StoreFileRequest(
                request.Cover,
                coverName,
                BucketNames.BooksCoverBucket,
                request.CoverContentType), cancellationToken);
        }

        var userBook = new UserBook(
            identityProvider.UserId,
            request.Title.Trim(),
            bookFile.Id,
            storedCover?.Id);

        dbContext.UserBooks.Add(userBook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static string GetEbookFileName(string fileName, string contentType)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = contentType.ToLowerInvariant() switch
        {
            "application/epub+zip" => ".epub",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{fileNameWithoutExtension}{extension}";
    }

    private static string GetCoverFileName(string fileName, string contentType)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{fileNameWithoutExtension}{extension}";
    }

    public static string GetRandomName()
    {
        return Guid.NewGuid().ToString("N");
    }
}
