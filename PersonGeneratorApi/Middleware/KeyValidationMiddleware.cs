using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace PersonGeneratorApi.Middleware
{
    public class KeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public KeyValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers["AccessKey"] == _configuration["ApiAccessKey"])
            {
                await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied!");
            }
        }
    }
}
