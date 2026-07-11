using Learnup.Application.Responses.Public.Conversations;
using Learnup.Domain.AggregateRoots.Conversations;

namespace Learnup.Application.Mappings;

public static class ConversationMappings
{
    public static ConversationResponse ToResponse(this Conversation conversation)
    {
        return new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.Description,
            conversation.Duration,
            true,
            conversation.Items
                .OrderBy(item => item.Order)
                .Select(item => item.ToResponse())
                .ToArray());
    }

    public static ConversationItemResponse ToResponse(this ConversationItem conversationItem)
    {
        return new ConversationItemResponse(
            conversationItem.Id,
            conversationItem.Content,
            conversationItem.Translation,
            conversationItem.Order,
            conversationItem.Person);
    }

    public static ConversationItemExpressionResponse ToResponse(this ConversationItemExpression expression)
    {
        return new ConversationItemExpressionResponse(
            expression.Id,
            expression.Phrase,
            expression.Meaning,
            expression.Translation);
    }
}
