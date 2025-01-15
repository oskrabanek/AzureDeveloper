using Azure.Messaging.EventHubs.Consumer;

var connectionString = ""; // "Endpoint=sb://evhns-azuredev00.servicebus.windows.net/;SharedAccessKeyName=Consumer;SharedAccessKey=************;EntityPath=evh-azuredev00";
var eventHubName = ""; //"evh-azuredev00";
var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

var consumer = new EventHubConsumerClient(
    consumerGroup,
    connectionString,
    eventHubName);

while (true)
{
    try
    {
        using CancellationTokenSource cancellationSource = new CancellationTokenSource();
        cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

        int eventsRead = 0;
        int maximumEvents = 3;

        await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(cancellationSource.Token))
        {
            string readFromPartition = partitionEvent.Partition.PartitionId;
            byte[] eventBodyBytes = partitionEvent.Data.EventBody.ToArray();

            Console.WriteLine($"Read event of length {eventBodyBytes.Length} from {readFromPartition}");
            eventsRead++;

            if (eventsRead >= maximumEvents)
            {
                break;
            }
        }
    }
    catch (TaskCanceledException)
    {
        // This is expected if the cancellation token is
        // signaled.
    }
    finally
    {
        //await consumer.CloseAsync();
    }

    Thread.Sleep(1000);
}

await consumer.CloseAsync();