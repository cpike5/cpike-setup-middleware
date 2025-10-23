using Microsoft.AspNetCore.Http;

namespace cpike.Setup.Middleware
{
    public class SetupMiddleware
    {

        private readonly RequestDelegate _next;

        public SetupMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Middleware logic goes here.
            // For example, you can log requests, modify the response, etc.
            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
