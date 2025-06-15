using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Models;
using System.Text.Json;

namespace GenderHealthCare.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorException ex)
            {
                _logger.LogWarning(ex, "Handled business exception");
                await HandleExceptionAsync(context, ex.StatusCode, ex.ErrorDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled server exception");
                var error = new ErrorDetail
                {
                    ErrorMessage = "An unexpected error occurred.",
                    ErrorCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                };
                await HandleExceptionAsync(context, (int)StatusCodeHelper.ServerError, error);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, ErrorDetail errorDetail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                status = statusCode,
                errorCode = errorDetail.ErrorCode,
                message = errorDetail.ErrorMessage
            });

            await context.Response.WriteAsync(result);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
