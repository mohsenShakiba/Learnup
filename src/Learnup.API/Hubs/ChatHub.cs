using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Learnup.API.Hubs;

/// <summary>
/// Real-time chat hub. Chat requests are submitted over HTTP; authenticated
/// connections receive queued chat progress events through this hub.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    public const string ChatStarted = "ChatStarted";
    public const string ChatDelta = "ChatDelta";
    public const string ChatCompleted = "ChatCompleted";
    public const string ChatFailed = "ChatFailed";

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        await base.OnConnectedAsync();
    }

    public static string UserGroup(int userId)
    {
        return $"user:{userId}:chats";
    }

    private int GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(value, out var userId)
            ? userId
            : throw new HubException("Authenticated user id was not found.");
    }
}
