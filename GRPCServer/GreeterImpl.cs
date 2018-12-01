
using System.Threading.Tasks;
using Grpc.Core;
using HelloWorld;

class GreeterImpl : Greeter.GreeterBase
{
    // Server side handler of the SayHello RPC
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
    }

    public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        for (int i = 0; i < 3; i++)
        {
            await responseStream.WriteAsync(new HelloReply { Message = $"How are you {request.Name}? {i}"});
            // Gotta look busy
            await Task.Delay(1000);
        }

        await responseStream.WriteAsync(new HelloReply { Message = $"Goodbye {request.Name}!"});
    }
}