using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Extensions;

public static class ErrorHandlerExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger("Error Handler");

            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionDetails?.Error;

            logger.LogError(exception, "Request can't be processed on machine {Machine}. TraceId: {TraceId}",
            Environment.MachineName,
            Activity.Current?.TraceId);

            var problem = new ProblemDetails
            {
                Title = "System is facing some challenge but we're working on it!",
                Status = StatusCodes.Status500InternalServerError,
                Extensions =
                {
                    {"traceId", Activity.Current?.TraceId.ToString()}
                }
            };

            var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

            if (environment.IsDevelopment())
            {
                problem.Detail = exception?.ToString();
            }

            await Results.Problem(problem).ExecuteAsync(context);
        });
    }
}