using CrmPlatformAPI.Errors;
using System.Net;
using System.Text.Json;

namespace CrmPlatformAPI.Middleware
{
    public class ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger, IHostEnvironment _environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _environment.IsDevelopment()
                    ? new Exceptions((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
                    : new Exceptions((int)HttpStatusCode.InternalServerError, "Internal Server Error", ex.Message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }   
    }
 
}
