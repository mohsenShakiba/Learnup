using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Ebooks;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record DeleteUserBook(int UserBookId) : IRequest;

internal sealed class DeleteUserBookHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<DeleteUserBook>
{
    public async Task<Unit> Handle(DeleteUserBook request, CancellationToken cancellationToken)
    {
        var userBook = await dbContext.UserBooks
            .Include(ub => ub.Ebook)
            .FirstOrDefaultAsync(
                ub => ub.Id == request.UserBookId && ub.UserId == identityProvider.UserId,
                cancellationToken);

        if (userBook is null)
        {
            return Unit.Value;
        }

        if (userBook.Ebook.Source == EbookSource.User)
        {
            dbContext.Ebooks.Remove(userBook.Ebook);
        }
        else
        {
            dbContext.UserBooks.Remove(userBook);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
