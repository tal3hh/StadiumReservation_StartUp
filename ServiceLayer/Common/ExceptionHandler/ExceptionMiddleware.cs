using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text;

namespace ServiceLayer.Common.ExceptionHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var method = httpContext.Request.Method;
                var path = httpContext.Request.Path;
                var query = httpContext.Request.QueryString;
                var body = await GetRequestBody(httpContext.Request);

                Log.Information($"Method: {method};" +
                                $"\nPath: {path};" +
                                $"\nQuery: {query};" +
                                $"\nBody: {body}");

                await _next.Invoke(httpContext);

            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync("Servisde bir xeta yarandi");
            }
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {

            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
