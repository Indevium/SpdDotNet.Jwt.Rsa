using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SpdDotNet.Jwt.Rsa.Lib
{
    public static class Responses
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task SendJson(HttpContext context, object data, int statusCode = StatusCodes.Status200OK)
        {
            var a = JsonSerializer.Serialize(data, Options);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(a, Encoding.UTF8);
        }

        public static async Task Response404RouteNotFound(HttpContext context) =>
            await SendJson(context, new { mvsErrorMessage = "Route not found" }, StatusCodes.Status404NotFound);

        public static async Task Response401Unauthorized(HttpContext context) =>
            await SendJson(context, new { mvsErrorMessage = "Unauthorized" }, StatusCodes.Status401Unauthorized);
    }
}
