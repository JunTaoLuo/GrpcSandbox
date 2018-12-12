using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HelloCounter;
using Microsoft.Extensions.Logging;

namespace GRPCServer
{
    public class CounterImpl : Counter.CounterBase
    {
        private ILogger _logger;
        private IncrementingCounter _counter;

        public CounterImpl(IncrementingCounter counter, ILoggerFactory loggerFactory)
        {
            _counter = counter;
            _logger = loggerFactory.CreateLogger<CounterImpl>();
        }

        public override Task<CounterReply> IncrementCount(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Incrementing count by 1");
            _counter.Increment(1);
            return Task.FromResult(new CounterReply { Count = _counter.Count });
        }

        public override async Task<CounterReply> AccumulateCount(IAsyncStreamReader<CounterRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                _logger.LogInformation($"Incrementing count by {requestStream.Current.Count}");

                _counter.Increment(requestStream.Current.Count);
            }

            return new CounterReply { Count = _counter.Count };
        }
    }
}
