using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRPCServer.Dotnet;
using Helloworld;

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
        public static IServiceCollection AddGrpc<GrpcImpl>(this IServiceCollection services) where GrpcImpl : Greeter.GreeterBase, new()
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceBinder = new ServiceBinder();
            Greeter.BindService(serviceBinder, new GrpcImpl());
            services.AddSingleton(serviceBinder);

            return services;
        }
    }
}
