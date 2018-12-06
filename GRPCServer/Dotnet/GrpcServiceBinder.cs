using System.Collections.Generic;
using Grpc.Core;
using GRPCServer.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace GRPCServer.Dotnet
{
    public class GrpcServiceBinder : ServiceBinderBase
    {
        private IEndpointRouteBuilder _routeBuilder;

        public GrpcServiceBinder(IEndpointRouteBuilder routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        internal IDictionary<string, IServerCallHandler> CallHandlers { get; } = new Dictionary<string, IServerCallHandler>();

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new UnaryServerCallHandler<TRequest, TResponse>(method, handler).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new ServerStreamingServerCallHandler<TRequest, TResponse>(method, handler).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new ClientStreamingServerCallHandler<TRequest, TResponse>(method, handler).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new DuplexStreamingServerCallHandler<TRequest, TResponse>(method, handler).HandleCallAsync);
        }
    }
}
