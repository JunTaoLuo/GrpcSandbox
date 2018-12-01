using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HelloCounter;

namespace GRPCServer
{
    public class CounterImpl : Counter.CounterBase
    {
        private int _count;

        public override Task<CounterReply> IncrementCount(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new CounterReply { Count = ++_count });
        }

        public override async Task<CounterReply> AccumulateCount(IAsyncStreamReader<CounterRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                _count += requestStream.Current.Count;
            }

            return new CounterReply { Count = _count };
        }
    }
}
