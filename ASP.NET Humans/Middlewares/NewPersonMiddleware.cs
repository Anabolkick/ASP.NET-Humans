using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASP.NET_Humans.Middlewares
{
    public class NewPersonMiddleware
    {
        private readonly RequestDelegate next;

        public NewPersonMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next(context);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseNewPerson(this IApplicationBuilder app)
        {
            return app.UseMiddleware<NewPersonMiddleware>();
        }
    }
}
