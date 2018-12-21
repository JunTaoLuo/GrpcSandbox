using System;
using Grpc.Core;
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

            var serviceBinder = new GrpcServiceBinder<TImplementation>(builder);

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

            // The method we want to call is public static void BindService(ServiceBinderBase serviceBinder, CounterBase serviceImpl)
            var bindService = declaringType.GetMethod("BindService", new[] { typeof(ServiceBinderBase), baseType });

            // Invoke
            // Note that the service binder API right now requires a non-null instance. Hopefully we can change it so we don't have to create an arbitrary instance here.
            bindService.Invoke(null, new object[] { serviceBinder, new DefaultGrpcServiceActivator<TImplementation>(builder.ServiceProvider).Create() });

            return builder;
        }
    }
}
