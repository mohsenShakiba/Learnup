using System.Security.Claims;
using Learnup.Application.Features.Public.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Learnup.API.Hubs;

/// <summary>
/// Real-time chat hub. Clients invoke <see cref="StreamReply"/> and consume the returned
/// stream to receive the assistant's reply token-by-token as it is generated.
/// </summary>
[Authorize]
public sealed class ChatHub(IChatStreamService chatStreamService) : Hub
{
    public IAsyncEnumerable<string> StreamReply(
        int conversationId,
        string message,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        return chatStreamService.StreamAsync(userId, conversationId, message, cancellationToken);
    }

    private int GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(value, out var userId)
            ? userId
            : throw new HubException("Authenticated user id was not found.");
    }
}
