using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Services;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public string GetConnectionId() => Context.ConnectionId;

    public async Task SendObject(string user, string callbackName, object obj)
    {
        _logger.LogInformation("Sending object to {User}", user);
        await Clients.Client(user).SendAsync(callbackName, obj);
    }
}
