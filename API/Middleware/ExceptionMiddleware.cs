using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                await next(context);
            }catch(Exception ex) {
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                APIException response = env.IsDevelopment() ? new APIException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) : new APIException(context.Response.StatusCode, "Internal server error");
                JsonSerializerOptions options = new JsonSerializerOptions {PropertyNamingPolicy=JsonNamingPolicy.CamelCase};
                string json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}