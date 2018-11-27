using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GRPCSample.Dotnet
{
    public class GrpcMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServiceBinder _serviceBinder;

        public GrpcMiddleware(RequestDelegate next, ServiceBinder serviceBinder)
        {
            _next = next;
            _serviceBinder = serviceBinder;
        }

        public Task Invoke(HttpContext httpContext)
        {
            _serviceBinder.CallHandlers.TryGetValue(httpContext.Request.Path, out var callHandler);

            return callHandler?.HandleCallAsync(httpContext) ?? _next(httpContext);
        }
    }
}
