using System;
using Grpc.Core;
using GRPCServer.Dotnet;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the ResponseCaching middleware.
    /// </summary>
    public static class GrpcServicesExtensions
    {
        /// <summary>
        /// Add response caching services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        public static IServiceCollection AddGrpc(this IServiceCollection services, Action<ServiceBinderBase> configureServiceBinder)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceBinder = new ServiceBinder();
            configureServiceBinder(serviceBinder);
            services.AddSingleton(serviceBinder);

            return services;
        }
    }
}
