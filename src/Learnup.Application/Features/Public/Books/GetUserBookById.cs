using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record GetUserBookById(int Id) : IRequest<UserBookResponse?>;

internal sealed class GetUserBookByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetUserBookById, UserBookResponse?>
{
    public async Task<UserBookResponse?> Handle(GetUserBookById request, CancellationToken cancellationToken)
    {
        var book = await dbContext.UserBooks
            .AsNoTracking()
            .Include(book => book.Ebook)
            .Where(book => book.Id == request.Id && book.UserId == identityProvider.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        return book?.ToResponse();
    }
}
