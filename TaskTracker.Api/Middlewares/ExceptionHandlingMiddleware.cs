namespace TaskTracker.Api.Middlewares;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Exceptions;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TaskTrackerException ex)
        {
            _logger.LogWarning(ex, "Обработано прикладное исключение");
            await WriteProblemDetailsAsync(context, ex.StatusCode, ex.Title, ex.Detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Непредвиденная ошибка");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Внутренняя ошибка",
                "Произошла непредвиденная ошибка. Повторите попытку позже.");
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
