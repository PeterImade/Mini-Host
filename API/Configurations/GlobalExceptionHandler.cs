using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace API.Configurations
{
    public class GlobalExceptionHandler: IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception caught by middleware.");

            var (status, title) = exception switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, "A required argument was null."),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            var problem = new
            {
                type = "https://httpstatuses.com/" + (int)status,
                title,
                status = (int)status,
                detail = exception.Message,
                traceId = context.TraceIdentifier
            };

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem), cancellationToken);

            return true; // handled
        }
    }
}
