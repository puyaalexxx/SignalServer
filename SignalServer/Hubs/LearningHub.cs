using Microsoft.AspNetCore.SignalR;

namespace SignalServer.Hubs;

public class LearningHub : Hub<ILearningHubClient>
{
    //send message to all clients
    public async Task BroadcastMessage(string message)
    {
        await Clients.All.ReceiveMessage(message);
    }

    //send to other clients
    public async Task SendToOthers(string message)
    {
        await Clients.Others.ReceiveMessage(message);
    }
    
    //send to self
    public async Task SendToCaller(string message)
    {
        await Clients.Caller.ReceiveMessage(GetMessageToSend(message));
    }
    
    //send to specific client
    public async Task SendToIndividual(string connectionId, string message)
    {
        await Clients.Client(connectionId).ReceiveMessage(GetMessageToSend(message));
        
        //send to a group of specific clients (ids)
        //await Clients.Clients(connectionIds).ReceiveMessage(GetMessageToSend(message));
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
    
    private string GetMessageToSend(string originalMessage)
    {
        return $"User connection id: {Context.ConnectionId}. Message: {originalMessage}";
    }
}