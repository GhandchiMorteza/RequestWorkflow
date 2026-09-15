using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RequestWorkflow.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        var problemDetailsService =
            httpContext.RequestServices
                .GetRequiredService<IProblemDetailsService>();

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,

                ProblemDetails = new ProblemDetails
                {
                    Status =
                        StatusCodes.Status500InternalServerError,

                    Title =
                        "An unexpected error occurred.",

                    Detail =
                        "The server encountered an unexpected error."
                }
            });
    }
}
