using GRPCServer.Dotnet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using HelloCounter;
using HelloWorld;

namespace GRPCServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(builder =>
            {
                builder.BindService(new CounterImpl());
                builder.BindService(new GreeterImpl());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ServiceBinder serviceBinder)
        {
            app.UseGrpc();

            app.Run(context => context.Response.WriteAsync("Hello world!"));
        }
    }
}
