using System;
using Grpc.Core;
using HelloCounter;

namespace GRPCClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Counter.CounterClient(channel);

            var reply = client.GetCount(new Google.Protobuf.WellKnownTypes.Empty());
            Console.WriteLine("Count: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
