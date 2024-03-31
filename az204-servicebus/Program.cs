using Azure.Messaging.ServiceBus;

string connection_string = "{connection-string}";
string queue_name = "{queue-name}";

ServiceBusClient client;

ServiceBusSender sender;

client = new ServiceBusClient(connection_string);
sender = client.CreateSender(queue_name);

using ServiceBusMessageBatch message_batch = await sender.CreateMessageBatchAsync();

for (int i = 1; i <= 3; i++)
{
    if(!message_batch.TryAddMessage(new ServiceBusMessage($"message {i}")))
    {
        throw new Exception($"exception {i} has occurred");
    }
}

try 
{
    await sender.SendMessagesAsync(message_batch);
    Console.WriteLine("message batch sent successfully");
}
finally
{
    // dispose of sender and client to ensure resources are properly cleaned up
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to exit");
Console.ReadLine();

ServiceBusProcessor processor;
client = new ServiceBusClient(connection_string);

// create a processor that we can use to process the messages
processor = client.CreateProcessor(queue_name, new ServiceBusProcessorOptions());

try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;

    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // start processing 
    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    // stop processing 
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // complete the message. messages is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}