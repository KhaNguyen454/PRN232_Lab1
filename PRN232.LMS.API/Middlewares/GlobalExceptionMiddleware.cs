using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                int statusCode = StatusCodes.Status500InternalServerError;
                string message = $"Lỗi hệ thống: {ex.Message}";

                if (ex is System.Collections.Generic.KeyNotFoundException)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                }
                else if (ex.GetType().Name.Contains("ParseException") || ex is ArgumentException)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    message = $"Lỗi tham số đầu vào: {ex.Message}";
                }
                else
                {
                    _logger.LogError(ex, "An unhandled exception occurred.");
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var response = ApiResponse<object>.Fail(message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
