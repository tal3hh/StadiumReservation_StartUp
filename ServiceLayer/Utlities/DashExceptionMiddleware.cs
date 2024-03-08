using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    public class DashExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public DashExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync("Servisce bir hata olustu");
            }
        }
    }
}
