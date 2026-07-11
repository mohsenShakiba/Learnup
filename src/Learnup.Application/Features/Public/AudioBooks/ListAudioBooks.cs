using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.AudioBooks;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.AudioBooks;

public sealed record ListAudioBooks : IRequest<IReadOnlyList<AudioBookResponse>>;

internal sealed class ListAudioBooksHandler(ILearnupDbContext dbContext)
    : IRequestHandler<ListAudioBooks, IReadOnlyList<AudioBookResponse>>
{
    public async Task<IReadOnlyList<AudioBookResponse>> Handle(
        ListAudioBooks request,
        CancellationToken cancellationToken)
    {
        var audioBooks = await dbContext.AudioBooks
            .AsNoTracking()
            .OrderBy(audioBook => audioBook.Title)
            .ThenBy(audioBook => audioBook.Id)
            .ToListAsync(cancellationToken);

        return audioBooks.Select(audioBook => audioBook.ToResponse()).ToList();
    }
}
