using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpdDotNet.Jwt.Rsa.Lib;

namespace SpdDotNet.Jwt.Rsa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting()
                .AddControllers();

            services.AddCors(o => o.AddPolicy(Defines.CorsPolicyName, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(Defines.CorsPolicyName);
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/healthcheck/get500", context => throw new Exception("Test response exception"));
                endpoints.MapGet("/healthcheck/get200", async context => await Responses.SendJson(context, new { message = "Ok" }));

                endpoints.MapControllers();
                endpoints.Map("{*path}", Responses.Response404RouteNotFound);
            });

            app.Run(Responses.Response404RouteNotFound);
        }
    }
}
