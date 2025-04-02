// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Please specifiy the URL of the SignalR Hub");

var url = Console.ReadLine();

var hubConnetion = new HubConnectionBuilder().WithUrl(url!).Build();

hubConnetion.On<string>("ReceiveMessage", message => Console.WriteLine($"SignalR Hub Message {message}"));

try
{
    await hubConnetion.StartAsync();

    while (true)
    {
        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("exit - Exit the program");
        
        var action = Console.ReadLine();
        
        Console.WriteLine("Please specify the message:");
        string? message = Console.ReadLine();

        if (action == "exit") break;
        
        await hubConnetion.SendAsync("BroadcastMessage", message);
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();

    return;
}