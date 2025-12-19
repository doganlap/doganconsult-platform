using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Validation;

namespace DoganConsult.Web.HttpApi.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. RequestId: {RequestId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";
        var details = (string?)null;

        // Handle ABP Framework exceptions
        if (exception is BusinessException businessException)
        {
            code = HttpStatusCode.BadRequest;
            message = businessException.Message;
            details = businessException.Details;
        }
        else if (exception is AbpAuthorizationException)
        {
            code = HttpStatusCode.Forbidden;
            message = "You are not authorized to perform this action.";
        }
        else if (exception is AbpValidationException validationException)
        {
            code = HttpStatusCode.BadRequest;
            message = "Validation failed.";
            details = string.Join("; ", validationException.ValidationErrors.Select(e => e.ErrorMessage));
        }
        // Note: AbpDbConcurrencyException may not be available in all ABP versions
        // Handle concurrency exceptions via DbUpdateConcurrencyException if needed

        var response = new
        {
            error = new
            {
                code = code.ToString(),
                message = message,
                details = details,
                data = new
                {
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                }
            }
        };

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
