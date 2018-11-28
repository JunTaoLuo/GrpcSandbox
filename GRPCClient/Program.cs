using System;
using Grpc.Core;
using Helloworld;

namespace GRPCClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            String user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
