using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record ListUserBooks : IRequest<IReadOnlyList<UserBookResponse>>;

internal sealed class ListUserBooksHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ListUserBooks, IReadOnlyList<UserBookResponse>>
{
    public async Task<IReadOnlyList<UserBookResponse>> Handle(ListUserBooks request, CancellationToken cancellationToken)
    {
        var books = await dbContext.UserBooks
            .AsNoTracking()
            .Include(book => book.Ebook)
            .Where(book => book.UserId == identityProvider.UserId)
            .OrderByDescending(book => book.Ebook.UploadedAt)
            .ToListAsync(cancellationToken);

        return books.Select(book => book.ToResponse()).ToList();
    }
}
