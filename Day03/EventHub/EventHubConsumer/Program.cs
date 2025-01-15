using Azure.Messaging.EventHubs.Consumer;

var connectionString = "Endpoint=sb://evhnsazdev00.servicebus.windows.net/;SharedAccessKeyName=listener;SharedAccessKey=******************;EntityPath=evhazdev00";
var eventHubName = "evhazdev00";
var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

while (true)
{
    try
    {
        await using var consumerClient = new EventHubConsumerClient(
            consumerGroup,
            connectionString,
            eventHubName);

        using CancellationTokenSource cancellationSource = new CancellationTokenSource();
        cancellationSource.CancelAfter(TimeSpan.FromSeconds(5));

        int eventsRead = 0;
        int maximumEvents = 3;

        var partitionIds = await consumerClient.GetPartitionIdsAsync();
        Console.WriteLine(string.Join(", ", partitionIds));

        var options = new ReadEventOptions
        {
            MaximumWaitTime = TimeSpan.FromSeconds(1)
        };

        await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync(startReadingAtEarliestEvent: true, readOptions: options, cancellationToken: cancellationSource.Token))
        {
            if (partitionEvent.Partition is null)
            {
                continue;
            }

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
        Console.WriteLine("The read operation was canceled. This is expected if the cancellation token is signaled.");
        // This is expected if the cancellation token is
        // signaled.
    }
    finally
    {
        //await consumer.CloseAsync();
    }

    Thread.Sleep(1000);
}
