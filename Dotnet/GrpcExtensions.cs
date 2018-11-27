using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRPCSample.Dotnet;

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
