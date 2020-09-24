using StackExchange.Redis;
using System;
using System.Configuration;
using Console = System.Console;

namespace Redis.Cmd
{
    class Program
    {
        private const string CacheConnection = "<REDIS CONNECTION STRING>";

        static void Main(string[] args)
        {
            // Connection refers to a property that returns a ConnectionMultiplexer
            // as shown in the previous example.
            IDatabase cache = Connection.GetDatabase();

            // Perform cache operations using the cache object...

            // Simple PING command
            string cacheCommand = "PING";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            // Simple get and put of integral data types into the cache
            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            cacheCommand = "SET Message \"Hello! The cache is working from a .NET console app!\"";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringSet()");
            Console.WriteLine("Cache response : " + cache.StringSet("Message", "Hello! The cache is working from a .NET console app!").ToString());

            // Demonstrate "SET Message" executed as expected...
            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            // Get the client list, useful to see if connection list is growing...
            // Note that this requires the allowAdmin=true
            cacheCommand = "CLIENT LIST";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            var endpoint = (System.Net.DnsEndPoint)Connection.GetEndPoints()[0];
            var server = Connection.GetServer(endpoint.Host, endpoint.Port);

            var clients = server.ClientList();
            Console.WriteLine("Cache response :");
            foreach (var client in clients)
            {
                Console.WriteLine(client.Raw);
            }

            lazyConnection.Value.Dispose();
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(()
            => ConnectionMultiplexer.Connect(CacheConnection + ",allowAdmin=true"));

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
