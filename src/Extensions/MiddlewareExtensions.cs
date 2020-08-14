using Microsoft.AspNetCore.Builder;
using WebUI.Middleware;

namespace WebUI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}