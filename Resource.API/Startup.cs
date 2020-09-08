using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpdDotNet.Jwt.Rsa.Lib;

namespace SpdDotNet.Jwt.Rsa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, TokenValidatorPostConfigure>();

            services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.IncludeErrorDetails = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidAudience = "TokenAudience",
                    };
                });

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

            app.UseAuthentication();
            app.UseAuthorization();

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
