using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record UpdateUserBookCurrentPage(int UserBookId, string CurrentRef, float? Progress) : IRequest;

internal sealed class UpdateUserBookCurrentPageHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<UpdateUserBookCurrentPage>
{
    public async Task<Unit> Handle(UpdateUserBookCurrentPage request, CancellationToken cancellationToken)
    {
        var book = await dbContext.UserBooks
            .FirstOrDefaultAsync(book => book.Id == request.UserBookId &&
                                         book.UserId == identityProvider.UserId, cancellationToken);

        if (book is null)
        {
            throw new InvalidOperationException("User book not found.");
        }

        book.UpdateCurrentRef(request.CurrentRef, request.Progress);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}