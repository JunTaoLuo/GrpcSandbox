using Grpc.Core;
using GRPCServer.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace GRPCServer.Dotnet
{
    public class GrpcServiceBinder<TImplementation> : ServiceBinderBase where TImplementation : class
    {
        private IEndpointRouteBuilder _routeBuilder;

        public GrpcServiceBinder(IEndpointRouteBuilder routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            // handler is null and will be dynamically activated via DI
            _routeBuilder.Map(method.FullName, new UnaryServerCallHandler<TRequest, TResponse, TImplementation>(method).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new ServerStreamingServerCallHandler<TRequest, TResponse, TImplementation>(method).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new ClientStreamingServerCallHandler<TRequest, TResponse, TImplementation>(method).HandleCallAsync);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
        {
            _routeBuilder.MapPost(method.FullName, new DuplexStreamingServerCallHandler<TRequest, TResponse, TImplementation>(method).HandleCallAsync);
        }
    }
}
