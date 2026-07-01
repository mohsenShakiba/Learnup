using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class StoryPipeline(ILearnupDbContext dbContext, IVoiceProvider voiceProvider, ILogger<StoryPipeline> logger) : IPipeline
{
    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var stories = await dbContext.Stories
            .Include(s => s.Items)
            .Where(s => s.Status == StoryStatus.Pending)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var story in stories)
        {
            foreach (var item in story.Items)
            {
                try
                {
                    var option = item.Person == 1 ? new VoiceOptions(VoiceIds.Heart, 0.8) : new VoiceOptions(VoiceIds.Bella, 0.8);
                    var result = await voiceProvider.GetVoiceAsync(item.Content, option, cancellationToken: cancellationToken);
                    item.SetVoice(result.VoiceId);
                }
                catch
                {
                    // do nothing
                }
            }

            story.MarkAsCompleted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
