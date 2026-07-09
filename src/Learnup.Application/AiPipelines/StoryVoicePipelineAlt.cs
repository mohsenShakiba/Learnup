using System.Text;
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
                // do nothing
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Concatenates the conversation's lines (in order) into a single block of text so the whole
    /// conversation can be voiced in one ElevenLabs call.
    /// </summary>
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
