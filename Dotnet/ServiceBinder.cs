using System.Collections.Generic;
using Grpc.Core;
using GRPCSample.Internal;

namespace GRPCSample.Dotnet
{
    public class ServiceBinder : ServiceBinderBase
    {
        internal IDictionary<string, IServerCallHandler> CallHandlers { get; } = new Dictionary<string, IServerCallHandler>();

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            CallHandlers.Add(method.FullName, new UnaryServerCallHandler<TRequest, TResponse>(method, handler));
        }
    }
}
