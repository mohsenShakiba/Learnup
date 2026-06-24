using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record GetUserBookById(int Id) : IRequest<UserBookResponse?>;

internal sealed class GetUserBookByIdHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetUserBookById, UserBookResponse?>
{
    public async Task<UserBookResponse?> Handle(
        GetUserBookById request,
        CancellationToken cancellationToken)
    {
        return await dbContext.UserBooks
            .AsNoTracking()
            .Where(book => book.Id == request.Id && book.UserId == identityProvider.UserId)
            .Select(book => new UserBookResponse(
                book.Id,
                book.Ebook.Title,
                book.Ebook.Author,
                book.Ebook.FileName,
                book.Ebook.CoverId,
                book.CurrentRef,
                book.Progress,
                book.Ebook.UploadedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
