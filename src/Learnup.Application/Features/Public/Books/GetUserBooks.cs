using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Books;

public sealed record GetUserBooks : IRequest<IReadOnlyList<UserBookResponse>>;

internal sealed class GetUserBooksHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetUserBooks, IReadOnlyList<UserBookResponse>>
{
    public async Task<IReadOnlyList<UserBookResponse>> Handle(
        GetUserBooks request,
        CancellationToken cancellationToken)
    {
        var result =  await dbContext.UserBooks
            .AsNoTracking()
            .Where(book => book.UserId == identityProvider.UserId)
            .OrderByDescending(book => book.UploadedAt)
            .Select(book => new UserBookResponse(
                book.Id,
                book.Title,
                book.FileName,
                book.CurrentPage,
                book.UploadedAt))
            .ToListAsync(cancellationToken);
        
        return result;
    }
}
