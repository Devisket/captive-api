
using Captive.Model.Dto;
using Captive.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Captive.Applications.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                CaptiveException ex => new ErrorResponse(
                    ex.Message,
                    ex.StatusCode,
                    GetErrorCode(ex)),

                DbUpdateException => new ErrorResponse(
                    "A database error occurred while processing your request.",
                    StatusCodes.Status500InternalServerError,
                    "DATABASE_ERROR"),

                KeyNotFoundException => new ErrorResponse(
                    "The requested resource was not found.",
                    StatusCodes.Status404NotFound,
                    "NOT_FOUND"),

                UnauthorizedAccessException => new ErrorResponse(
                    "You are not authorized to access this resource.",
                    StatusCodes.Status401Unauthorized,
                    "UNAUTHORIZED"),

                _ => new ErrorResponse(
                    "An internal server error occurred.",
                    StatusCodes.Status500InternalServerError,
                    "INTERNAL_SERVER_ERROR")
            };

            response.StatusCode = errorResponse.StatusCode;

            // Log the full exception details for debugging
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            // Return only the sanitized error response to the client
            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }

        private string GetErrorCode(CaptiveException exception)
        {
            // Convert the exception name to an error code
            var exceptionName = exception.GetType().Name.Replace("Exception", "");
            return string.Join("_", exceptionName.Split(new[] { ' ' })).ToUpper();
        }
    }
}
