using Learnup.Application.Responses.Public.Stories;
using Learnup.Domain.AggregateRoots.Stories;

namespace Learnup.Application.Mappings;

public static class StoryMappings
{
    public static StoryResponse ToResponse(this Story story)
    {
        return new StoryResponse(
            story.Id,
            story.Title,
            story.CoverId,
            story.Items
                .OrderBy(item => item.Order)
                .Select(item => item.ToResponse())
                .ToArray());
    }

    public static StoryItemResponse ToResponse(this StoryItem storyItem)
    {
        return new StoryItemResponse(
            storyItem.Id,
            storyItem.Content,
            storyItem.Translation,
            storyItem.Order,
            storyItem.VoiceId,
            storyItem.Timestamps
                .OrderBy(timestamp => timestamp.Start)
                .Select(timestamp => timestamp.ToResponse())
                .ToArray());
    }

    public static StoryItemTimestampResponse ToResponse(this StoryItemTimestamp timestamp)
    {
        return new StoryItemTimestampResponse(
            timestamp.Id,
            timestamp.Word,
            timestamp.Start,
            timestamp.End);
    }
}
