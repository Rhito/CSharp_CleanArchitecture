using CleanArchitecture.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CleanArchitecture.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env; // 🌐 Thêm biến môi trường

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger,
                                   IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // ⚡ Cho phép yêu cầu đi tiếp qua các lớp khác (Controller, Service...)
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // 🚨 Nếu có bất kỳ lỗi nào xảy ra ở các lớp sau, nó sẽ "rơi" vào đây
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. Xác định giá trị mặc định (Lỗi hệ thống 500)
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            string? detailed = _env.IsDevelopment() ? exception.StackTrace : "Vui lòng liên hệ quản trị viên.";

            // 2. Kiểm tra nếu là lỗi do chúng ta chủ động định nghĩa
            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode; // Lấy 400, 404, hoặc 409 từ Exception con
                message = appEx.Message;
            }
            else
            {
                // Ghi log lại những lỗi bất ngờ (không phải AppException)
                _logger.LogError(exception, "System Error: {Msg}", exception.Message);
            }

            // 3. Cập nhật mã trạng thái thực tế cho HTTP Response 🎯
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // 4. Tạo phản hồi JSON
            var response = new ErrorDetails(statusCode, message, detailed);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
    public class ErrorDetails(int statusCode, string message, string? detailed = null)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public string? Detailed { get; set; } = detailed;
    }
}
