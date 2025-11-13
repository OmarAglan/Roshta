using Microsoft.AspNetCore.Diagnostics;
using Rosheta.Core.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Rosheta.Presentation.Middleware
{
    /// <summary>
    /// Global exception handling middleware for centralized error processing
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var (status, title, detail) = exception switch
            {
                ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    "Validation Error",
                    validationEx.Message
                ),
                NotFoundException notFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    "Resource Not Found",
                    notFoundEx.Message
                ),
                BusinessRuleException businessEx => (
                    (int)HttpStatusCode.UnprocessableEntity,
                    "Business Rule Violation",
                    businessEx.Message
                ),
                InfrastructureException infraEx => (
                    (int)HttpStatusCode.ServiceUnavailable,
                    "Service Unavailable",
                    _env.IsDevelopment() ? infraEx.Message : "A service error occurred. Please try again later."
                ),
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "Internal Server Error",
                    _env.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again later."
                )
            };

            response.StatusCode = status;

            var problemDetails = new
            {
                type = $"https://httpstatuses.com/{status}",
                title,
                status,
                detail,
                instance = context.Request.Path,
                timestamp = DateTime.UtcNow
            };

            // For Razor Pages (non-API), redirect to error page
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                context.Items["Exception"] = exception;
                context.Items["ErrorTitle"] = title;
                context.Items["ErrorMessage"] = detail;
                
                // Redirect to error page
                context.Response.Redirect($"/Error?statusCode={status}");
                return;
            }

            // For API endpoints, return JSON
            await response.WriteAsJsonAsync(problemDetails);
        }
    }
}