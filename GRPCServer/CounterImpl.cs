using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HelloCounter;

namespace GRPCServer
{
    public class CounterImpl : Counter.CounterBase
    {
        private int _count;

        public override Task<CounterReply> GetCount(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new CounterReply { Message = _count++ });
        }
    }
}
