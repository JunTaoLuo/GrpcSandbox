using System;
using Google.Protobuf.Reflection;
using GRPCServer.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace GRPCServer.Dotnet
{
    public static class GrpcRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapGrpcService<TImplementation>(this IEndpointRouteBuilder builder) where TImplementation : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            //var serviceBinder = new GrpcServiceBinder<TImplementation>(builder);

            // Get implementation type
            var implementationType = typeof(TImplementation);

            // Implementation type FooImpl derives from Foo.FooBase (with implicit base type of Object).
            var baseType = implementationType.BaseType;
            while (baseType.BaseType?.BaseType != null)
            {
                baseType = baseType.BaseType;
            }

            // We need to call Foo.BindService from the declaring type.
            var declaringType = baseType.DeclaringType;

            // Get the descriptor
            var descriptor = declaringType.GetProperty("Descriptor").GetValue(declaringType) as ServiceDescriptor ?? throw new InvalidOperationException("Cannot retrive service descriptor");

            foreach (var method in descriptor.Methods)
            {
                if (method.IsClientStreaming && method.IsServerStreaming)
                {
                    builder.MapPost($"{method.Service.FullName}/{method.Name}", new DuplexStreamingServerCallHandler<TImplementation>(method.InputType, method.OutputType, method.Name).HandleCallAsync);
                }
                else if (method.IsClientStreaming)
                {
                    builder.MapPost($"{method.Service.FullName}/{method.Name}", new ClientStreamingServerCallHandler<TImplementation>(method.InputType, method.Name).HandleCallAsync);
                }
                else if (method.IsServerStreaming)
                {
                    builder.MapPost($"{method.Service.FullName}/{method.Name}", new ServerStreamingServerCallHandler<TImplementation>(method.InputType, method.OutputType, method.Name).HandleCallAsync);
                }
                else
                {
                    builder.MapPost($"{method.Service.FullName}/{method.Name}", new UnaryServerCallHandler<TImplementation>(method.InputType, method.Name).HandleCallAsync);
                }
            }

            return builder;
        }
    }
}
