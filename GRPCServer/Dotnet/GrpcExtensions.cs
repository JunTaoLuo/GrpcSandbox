using System;
using GRPCServer.Dotnet;

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

            return app.UseMiddleware<GrpcMiddleware>();
        }
    }
}
