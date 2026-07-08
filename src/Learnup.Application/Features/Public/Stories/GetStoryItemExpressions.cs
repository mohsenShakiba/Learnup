using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Stories;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Stories;

public sealed record GetStoryItemExpressions(int StoryId, int StoryItemId)
    : IRequest<IReadOnlyList<StoryItemExpressionResponse>?>;

internal sealed class GetStoryItemExpressionsHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetStoryItemExpressions, IReadOnlyList<StoryItemExpressionResponse>?>
{
    public async Task<IReadOnlyList<StoryItemExpressionResponse>?> Handle(
        GetStoryItemExpressions request,
        CancellationToken cancellationToken)
    {
        var storyItem = await dbContext.Stories
            .AsNoTracking()
            .Where(story => story.Id == request.StoryId)
            .SelectMany(story => story.Items)
            .Include(item => item.Expressions)
            .FirstOrDefaultAsync(item => item.Id == request.StoryItemId, cancellationToken);

        return storyItem?.Expressions
            .Select(expression => expression.ToResponse())
            .ToArray();
    }
}
