using System;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace StorageQueue
{
    class Program
    {
        const string connectionString = "<connectionString>";
        const string queueName = "<queueName>";

        static void Main(string[] args)
        {
            // Get the connection string from app settings

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            // Create the queue
            queueClient.CreateIfNotExists();

            // 
            if (queueClient.Exists())
            {
                // Peek at the next message
                PeekedMessage[] peekedMessages = queueClient.PeekMessages(1);

                // Display the message
                Console.WriteLine($"Peeked message: '{peekedMessages[0].MessageText}'");


                // Get the message from the queue
                QueueMessage[] retrievedMessage = queueClient.ReceiveMessages(1);

                // Update the message contents
                queueClient.UpdateMessage(retrievedMessage[0].MessageId,
                        retrievedMessage[0].PopReceipt,
                        $"Updated contents {DateTime.UtcNow}",
                        TimeSpan.FromSeconds(5)  // Make it invisible for another 10 seconds
                    );

                // Get the message from the queue
                retrievedMessage = queueClient.ReceiveMessages(1);

                // Delete the message
                queueClient.DeleteMessage(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
            }
        }
    }
}
