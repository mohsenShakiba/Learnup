using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Stories;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Stories;

public sealed record GetStoryById(int Id) : IRequest<StoryResponse?>;

internal sealed class GetStoryByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetStoryById, StoryResponse?>
{
    public async Task<StoryResponse?> Handle(
        GetStoryById request,
        CancellationToken cancellationToken)
    {
        var story = await dbContext.Stories
            .AsNoTracking()
            .Include(story => story.Items)
            .FirstOrDefaultAsync(story => story.Id == request.Id, cancellationToken);

        return story?.ToResponse();
    }
}
