namespace SignalServer.Hubs;

public interface ILearningHubClient
{
    Task ReceiveMessage(string message);
}