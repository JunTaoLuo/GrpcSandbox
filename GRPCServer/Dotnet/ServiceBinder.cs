using System.Collections.Generic;
using Grpc.Core;
using GRPCServer.Internal;

namespace GRPCServer.Dotnet
{
    public class ServiceBinder : ServiceBinderBase
    {
        internal IDictionary<string, IServerCallHandler> CallHandlers { get; } = new Dictionary<string, IServerCallHandler>();

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            CallHandlers.Add(method.FullName, new UnaryServerCallHandler<TRequest, TResponse>(method, handler));
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
        {
            CallHandlers.Add(method.FullName, new ServerStreamingServerCallHandler<TRequest, TResponse>(method, handler));
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
        {
            CallHandlers.Add(method.FullName, new ClientStreamingServerCallHandler<TRequest, TResponse>(method, handler));
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
        {
            CallHandlers.Add(method.FullName, new DuplexStreamingServerCallHandler<TRequest, TResponse>(method, handler));
        }
    }
}
