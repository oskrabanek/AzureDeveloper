using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

var connectionString = "Endpoint=sb://evhnsazuredev00.servicebus.windows.net/;SharedAccessKeyName=publisher;SharedAccessKey=GF+wHFAQ17u5McPl6eT/j9A1BkO9bkDTMjJ07rQTad8="; // Endpoint=sb://evhnsazuredev00.servicebus.windows.net/;SharedAccessKeyName=publisher;SharedAccessKey=GF+wHFAQ17u5McPl6eT/j9A1BkO9bkDTMjJ07rQTad8=;EntityPath=evhazuredev00
var eventHubName = "evhazuredev00";

var producer = new EventHubProducerClient(connectionString, eventHubName);

var i = 0;
while (true)
{
    try
    {
        using EventDataBatch eventBatch = await producer.CreateBatchAsync();

        var content = $"This is an event body {i++}";
        var eventData = new EventData(content);

        if (!eventBatch.TryAdd(eventData))
        {
            throw new Exception($"The event could not be added.");
        }

        Console.WriteLine(content);
    }
    finally
    {
        //await producer.CloseAsync();
    }

    Thread.Sleep(1000);
}

await producer.CloseAsync();