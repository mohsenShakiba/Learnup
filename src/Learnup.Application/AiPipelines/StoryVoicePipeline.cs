using System.Text;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.AiPipelines;

public class StoryVoicePipeline(ILearnupDbContext dbContext, IVoiceProvider voiceProvider) : IPipeline
{
    private const double PlaybackSpeed = 0.9;

    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var stories = await dbContext.Stories
            .Include(s => s.Items)
            .Where(s => !s.Status.HasFlag(StoryStatus.Voiced))
            .Where(s => s.Id == 1)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var story in stories)
        {
            try
            {
                var conversation = BuildConversationText(story);
                if (string.IsNullOrWhiteSpace(conversation))
                {
                    continue;
                }

                var option = new VoiceOptions(ElevenLabsVoiceIds.Brian, PlaybackSpeed);
                var result = await voiceProvider.GetConversationVoiceAsync(conversation, option, cancellationToken);

                story.SetVoice(result.AudioFileId);
                story.MarkAsVoiced();
            }
            catch
            {
                await Task.Delay(5000);
                // do nothing
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildConversationText(Story story)
    {
        var builder = new StringBuilder();

        foreach (var item in story.Items.OrderBy(i => i.Order))
        {
            if (string.IsNullOrWhiteSpace(item.Content))
            {
                continue;
            }

            builder.AppendLine(item.Content.Trim());
        }

        return builder.ToString().Trim();
    }
}
