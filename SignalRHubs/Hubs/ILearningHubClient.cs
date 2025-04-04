namespace SignalRHubs.Hubs;

public interface ILearningHubClient
{
    Task ReceiveMessage(string message);
}