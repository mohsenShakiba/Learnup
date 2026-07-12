using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.AiPipelines;

public class ConversationVoicePipeline(ILearnupDbContext dbContext, IVoiceProvider voiceProvider) : IPipeline
{
    // Two-speaker conversations: odd-numbered speakers get a male voice, the rest a female voice.
    private const string MaleVoiceId = ElevenLabsVoiceIds.Brian;
    private const string FemaleVoiceId = ElevenLabsVoiceIds.Sarah;

    public bool Enabled => false;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var conversations = await dbContext.Conversations
            .Include(s => s.Items)
            .Where(s => !s.Status.HasFlag(ConversationStatus.Voiced))
            .Where(s => s.Id == 1)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            try
            {
                var turns = BuildConversationTurns(conversation);
                if (turns.Count == 0)
                {
                    continue;
                }

                var result = await voiceProvider.GetConversationVoiceAsync(turns, cancellationToken);

                conversation.SetVoice(result.AudioFileId);
                conversation.MarkAsVoiced();
            }
            catch
            {
                await Task.Delay(5000);
                // do nothing
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<VoiceTurn> BuildConversationTurns(Conversation conversation)
    {
        var turns = new List<VoiceTurn>();

        foreach (var item in conversation.Items.OrderBy(i => i.Order))
        {
            if (string.IsNullOrWhiteSpace(item.Content))
            {
                continue;
            }

            // Person alternates between 1 and 2; odd speakers are male, even speakers female.
            var voiceId = item.Person % 2 == 1 ? MaleVoiceId : FemaleVoiceId;
            turns.Add(new VoiceTurn(item.Content.Trim(), voiceId));
        }

        return turns;
    }
}
