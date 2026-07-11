using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.AudioBooks;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.AudioBooks;

public sealed record GetAudioBookById(int Id) : IRequest<AudioBookDetailResponse?>;

internal sealed class GetAudioBookByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetAudioBookById, AudioBookDetailResponse?>
{
    public async Task<AudioBookDetailResponse?> Handle(
        GetAudioBookById request,
        CancellationToken cancellationToken)
    {
        var audioBook = await dbContext.AudioBooks
            .AsNoTracking()
            .AsSplitQuery()
            .Include(audioBook => audioBook.Items)
            .ThenInclude(item => item.Expressions)
            .FirstOrDefaultAsync(audioBook => audioBook.Id == request.Id, cancellationToken);

        return audioBook?.ToDetailResponse();
    }
}
