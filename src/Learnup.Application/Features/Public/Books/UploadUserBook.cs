using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Features.Public.Books;

public sealed record UploadUserBook(
    Stream Content,
    string FileName,
    string? ContentType,
    long Length,
    string Title) : IRequest;

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

        var storedFile = await fileService.StoreAsync(new StoreFileRequest(
            request.Content,
            request.FileName,
            BucketNames.BooksBucket,
            request.ContentType), cancellationToken);

        var userBook = new UserBook(
            identityProvider.UserId,
            request.Title.Trim(),
            storedFile.Id);

        dbContext.UserBooks.Add(userBook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}