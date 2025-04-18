using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using SignalRHubs.Hubs;

namespace SignalRHubs.Hubs;

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
    
    //send to a group
    public async Task SendToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).ReceiveMessage(GetMessageToSend(message));
    }
    public async Task AddUserToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.ReceiveMessage($"Current user added to {groupName} group");
        await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} added to {groupName} group");
    }

    public async Task RemoveUserFromGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.ReceiveMessage($"Current user removed from {groupName} group");
        await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} removed from {groupName} group");
    }

    public async Task BroadcastStream(IAsyncEnumerable<string> stream)
    {
        await foreach (var item in stream)
        {
            await Clients.Caller.ReceiveMessage($"Server received {item}");
        }
    }
    
    
    /////////// other methods
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
    
    //server streaming
    public async IAsyncEnumerable<string> TriggerStream(int jobsCount, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < jobsCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            yield return $"Job {i} executed succesfully";
            
            await Task.Delay(1000, cancellationToken);
        }
    }
}