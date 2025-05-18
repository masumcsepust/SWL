using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _users = new();
    // remote connect string such as database
    //private readonly ApplicationDbAontext _dbcontext;
    public ChatHub()//(ApplicationDbAontext dbContext)
    {
        // _dbcontext = dbContext;
    }
    public override Task OnConnectedAsync()
    {
        var username = Context.GetHttpContext()?.Request.Query["username"];
        if (!string.IsNullOrEmpty(username))
        {
            _users[username] = Context.ConnectionId;
        }
        return base.OnConnectedAsync();
    }

    public async Task SendPrivateMessage(string toUser, string message)
    {
        if (_users.TryGetValue(toUser, out var connectionId))
        {
            var fromUser = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", fromUser, message);
        }
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var user = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (!string.IsNullOrEmpty(user))
        {
            _users.Remove(user);
        }
        return base.OnDisconnectedAsync(exception);
    }
}
