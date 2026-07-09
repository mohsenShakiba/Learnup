using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.AiPipelines;

public class StoryVoicePipelineAlt(ILearnupDbContext dbContext, IElevenLabsVoiceProvider voiceProvider) : IPipeline
{
    private const double PlaybackSpeed = 0.9;

    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var stories = await dbContext.Stories
            .Include(s => s.Items)
            .Where(s => s.Status == StoryStatus.Translated && s.Id == 1)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var story in stories)
        {
            foreach (var item in story.Items)
            {
                try
                {
                    var voiceId = item.Person == 1 ? ElevenLabsVoiceIds.Brian : ElevenLabsVoiceIds.Lily;
                    var option = new VoiceOptions(voiceId, PlaybackSpeed);
                    var result = await voiceProvider.GetVoiceAsync(item.Content, option, cancellationToken: cancellationToken);
                    item.SetVoice(result.VoiceId);

                    if (result.Sentences is { Count: > 0 })
                    {
                        item.SetVoiceTimings(result.Sentences.Select(s => (s.Text, s.Start, s.End)));
                    }
                }
                catch
                {
                    // do nothing
                }
            }

            story.MarkAsVoiced();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
