using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

var connectionString = "Endpoint=sb://evhnsazdev00.servicebus.windows.net/;SharedAccessKeyName=sender;SharedAccessKey=*****************;EntityPath=evhazdev00";
var eventHubName = "evhazdev00";

var i = 0;
while (true)
{
    try
    {
        var producer = new EventHubProducerClient(connectionString, eventHubName);
        using EventDataBatch eventBatch = await producer.CreateBatchAsync();

        var content = $"This is an event body {i++}";
        var eventData = new EventData(content);

        if (!eventBatch.TryAdd(eventData))
        {
            throw new Exception($"The event could not be added.");
        }

        await producer.SendAsync(eventBatch);

        Console.WriteLine(content);
    }
    catch (Exception ex)
    {
        Console.WriteLine("error: " + ex.Message);
    }
    finally
    {
        //await producer.CloseAsync();
    }

    Thread.Sleep(1000);
}
