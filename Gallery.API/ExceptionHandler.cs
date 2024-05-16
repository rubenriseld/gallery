using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.API
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;
        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var result = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            switch (exception)
            {
                case KeyNotFoundException:
                    result.Status = StatusCodes.Status404NotFound;
                    result.Title = "Not Found";
                    break;

                case ArgumentNullException:
                    result.Status = StatusCodes.Status400BadRequest;
                    result.Title = "Bad Request";
                    break;

                default:
                    result.Status = StatusCodes.Status500InternalServerError;
                    result.Title = "Internal Server Error";
                    break;
            }
            httpContext.Response.StatusCode = (int)result.Status;
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);

            _logger.LogWarning(exception.ToString());

            return true;
        }
    }
}
