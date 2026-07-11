using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record GetChat(int ChatId) : IRequest<ChatDetailResponse?>;

internal sealed class GetChatHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetChat, ChatDetailResponse?>
{
    public async Task<ChatDetailResponse?> Handle(GetChat request, CancellationToken cancellationToken)
    {
        var userId = identityProvider.UserId;

        return await dbContext.Chats
            .AsNoTracking()
            .Where(chat => chat.Id == request.ChatId && chat.UserId == userId)
            .Select(chat => new ChatDetailResponse(
                chat.Id,
                chat.Title,
                chat.CreatedAt,
                chat.UpdatedAt,
                chat.Messages
                    .OrderBy(message => message.Id)
                    .Select(message => new ChatMessageResponse(
                        message.Id,
                        message.Role.ToString(),
                        message.Content,
                        message.CreatedAt))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
