// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Text;
using ElasticKafka.Client.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Welcome to ElasticKafka Messaging!");
Console.WriteLine();


var success = await TryEstablishSignalRConnectionAsync();
if (success is false)
    return;

Console.WriteLine("Connection established, you can proceed to work.");
Console.WriteLine();

Console.WriteLine("You may input your message and then send it with \"Enter\"");
//Console.WriteLine("You may press \"Shift+Backspace\" to completely erase your message.");
Console.WriteLine("You may press \"F1\" to exit application.");
Console.WriteLine();

StringBuilder input = new();

while (true)
{
    var key = Console.ReadKey();
    switch (key.Key)
    {
        case ConsoleKey.F1:
            ClearLine();
            Console.WriteLine("Exiting");
            return;
        
        // case ConsoleKey.Backspace
        //     when key.Modifiers == ConsoleModifiers.Shift:
        //     input.Clear();
        //     ClearLine();
        //     break;
        
        case ConsoleKey.Backspace:
            if (input.Length is 0)
                break;
            
            input.Remove(input.Length - 1, 1);
            ClearChar();
            break;
        
        case ConsoleKey.Enter:
            await SendMessageAsync();
            break;
        
        default:
            input.Append(key.KeyChar);
            break;
    }
}


async Task<bool> TryEstablishSignalRConnectionAsync()
{
    Console.WriteLine("Establishing connection to MessagesHub...");
    
    string groupId = "MessageCreatedEventReceivers";

    var baseUrl = GatewayUrls.Get();
    string hubUrl = $"{baseUrl}/messagesHub";

    var connection = new HubConnectionBuilder()
        .WithUrl(hubUrl)
        .WithAutomaticReconnect() // Enables auto-reconnect
        .Build();

    connection.On<string>("MessageCreated", messageId =>
    {
        Console.WriteLine($"[Message created]: ID = {messageId}");
        Console.WriteLine();
    });

    try
    {
        await connection.StartAsync();
        Console.WriteLine($"Connected to SignalR Messages Hub, url: {hubUrl}");

        await connection.InvokeAsync("JoinGroup", groupId);
        Console.WriteLine($"Joined group: {groupId}");
        
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
        return false;
    }
}

async Task SendMessageAsync()
{
    if (input.Length is 0)
    {
        Console.WriteLine("Your message is empty and won't be sent.");
        return;
    }
    
    var message = input.ToString();
    input.Clear();
    
    var baseUrl = GatewayUrls.Get();
    var url = new Uri($"{baseUrl}/api/messages");

    Console.WriteLine($"Sending message [{message}], url: {url}");

    var request = new SendMessageRequest(message);
    
    var httpClientFactory = GlobalServiceProvider.GetService<IHttpClientFactory>();

    var client = httpClientFactory.CreateClient();
    
    await client.PostAsJsonAsync(url, request);
    //await Task.Yield();

    Console.WriteLine("Message sent successfully!");
    Console.WriteLine();
}

static void ClearLine(){
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth)); 
    Console.SetCursorPosition(0, Console.CursorTop - 1);
}

static void ClearChar()
{
    var current = Console.CursorLeft;
    
    Console.SetCursorPosition(current, Console.CursorTop);
    Console.Write(new string(' ', 1)); 
    Console.SetCursorPosition(current, Console.CursorTop);
}