using System;
using GRPCServer.Dotnet;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the Grpc services.
    /// </summary>
    public static class GrpcServicesExtensions
    {
        /// <summary>
        /// Add a GRPC service implementation.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcService<TImplementation>(this IServiceCollection services) where TImplementation : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO: routing options?
            services.AddRouting();
            services.AddSingleton<TImplementation>();
            services.AddSingleton<IGrpcService, GrpcService<TImplementation>>();

            return services;
        }
    }
}
