using Microsoft.AspNetCore.Http;

namespace ServiceLayer.Utlities
{
    public class WebExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public WebExceptionMiddleware(RequestDelegate next)
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
