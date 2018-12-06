using System;
using Grpc.Core;
using GRPCServer.Dotnet;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class GrpcExtensions
    {
        public static IApplicationBuilder UseGrpc(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var grpcServices = app.ApplicationServices.GetServices<IGrpcService>();

            app.UseEndpointRouting(routeBuilder =>
            {
                var serviceBinder = new GrpcServiceBinder(routeBuilder);

                foreach (var service in grpcServices)
                {
                    // Implementation type FooImpl derives from Foo.FooBase. We need to call Foo.BindService.
                    var implementationType = service.ImplementationType;
                    var baseType = implementationType.BaseType; // TODO: recursive?
                    var declaringType = baseType.DeclaringType;

                    // The method we want to call is public static void BindService(ServiceBinderBase serviceBinder, CounterBase serviceImpl)
                    var bindService = declaringType.GetMethod("BindService", new[] { typeof(ServiceBinderBase), baseType });

                    // Invoke
                    bindService.Invoke(null, new object[] { serviceBinder, app.ApplicationServices.GetRequiredService(implementationType) });
                }
            });

            return app.UseEndpoint();
        }
    }
}
