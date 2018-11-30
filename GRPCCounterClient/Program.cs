using System;
using System.IO;
using Common;
using Grpc.Core;
using HelloCounter;

namespace GRPCClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new Channel("localhost:50051", Utils.ClientSslCredentials);
            var client = new Counter.CounterClient(channel);

            var reply = client.GetCount(new Google.Protobuf.WellKnownTypes.Empty());
            Console.WriteLine("Count: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
