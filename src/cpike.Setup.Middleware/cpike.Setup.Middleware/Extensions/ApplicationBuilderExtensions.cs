using Microsoft.AspNetCore.Builder;

namespace cpike.Setup.Middleware.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSetupMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SetupMiddleware>();
        }
    }
}
