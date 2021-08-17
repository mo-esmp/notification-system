using Microsoft.AspNetCore.Builder;
using WebApi.Infrastructure.Middlewares;

namespace WebApi.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ApiExceptionHandlingMiddleware>();
    }
}