
using System.Threading.Tasks;
using Grpc.Core;
using HelloWorld;
using Microsoft.Extensions.Logging;

class GreeterImpl : Greeter.GreeterBase
{
    private ILogger _logger;

    public GreeterImpl(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GreeterImpl>();
    }

    //Server side handler of the SayHello RPC
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Sending hello to {request.Name}");
        return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
    }

    public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        for (int i = 0; i < 3; i++)
        {
            var message = $"How are you {request.Name}? {i}";
            _logger.LogInformation($"Sending greeting {message}");
            await responseStream.WriteAsync(new HelloReply { Message = message });
            // Gotta look busy
            await Task.Delay(1000);
        }

        _logger.LogInformation("Sending goodbye");
        await responseStream.WriteAsync(new HelloReply { Message = $"Goodbye {request.Name}!" });
    }
}