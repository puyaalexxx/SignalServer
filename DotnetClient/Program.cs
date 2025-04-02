// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Please specifiy the URL of the SignalR Hub");

var url = Console.ReadLine();

var hubConnetion = new HubConnectionBuilder().WithUrl(url!).Build();

hubConnetion.On<string>("ReceiveMessage", message => Console.WriteLine($"SignalR Hub Message {message}"));

try
{
    await hubConnetion.StartAsync();

    var running = true;

    while (running)
    {
        var groupName = string.Empty;
        
        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("1 - send to others");
        Console.WriteLine("2 - send to self");
        Console.WriteLine("3 - send to individual");
        Console.WriteLine("4 - send to a group");
        Console.WriteLine("5 - add user to a group");
        Console.WriteLine("6 - remove user from a group");
        Console.WriteLine("exit - Exit the program");
        
        var action = Console.ReadLine();
        
        Console.WriteLine("Please specify the message:");
        string? message = Console.ReadLine();
        
        if (action == "4" || action == "5" || action == "6") {
            Console.WriteLine("Please specify the group name:");
            groupName = Console.ReadLine();
        }

        switch (action)
        {
            case "0": 
                await hubConnetion.SendAsync("BroadcastMessage", message);
                break;
            case "1":
                await hubConnetion.SendAsync("SendToOthers", message);
                break;
            case "2":
                await hubConnetion.SendAsync("SendToCaller", message);
                break;
            case "3":
                Console.WriteLine("Please specify the connection id:");
                var connectionId = Console.ReadLine();
                await hubConnetion.SendAsync("SendToIndividual", connectionId, message);
                break;
            case "4":
                hubConnection.SendAsync("SendToGroup", groupName, message).Wait();
                break;
            case "5":
                hubConnection.SendAsync("AddUserToGroup", groupName).Wait(); 
                break;
            case "6":
                hubConnection.SendAsync("RemoveUserFromGroup", groupName).Wait();
                break;
            case "exit":
                running = false;
                break;
            default:
                Console.WriteLine("Invalid action specified");
                break;
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();

    return;
}