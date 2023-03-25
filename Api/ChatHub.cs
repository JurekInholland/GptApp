using Microsoft.AspNetCore.SignalR;

namespace Api;

public class ChatHub : Hub
{
    public async Task SendObject(string user, string callbackName, object obj)
    {
        await Clients.Client(user).SendAsync(callbackName, obj);
    }
}
