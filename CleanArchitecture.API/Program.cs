using CleanArchitecture.API.Middlewares;
using CleanArchitecture.Application.DTOs.Validation;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Service;
using CleanArchitecture.Infrastructure.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 1. QUAN TRỌNG NHẤT: Cho phép dấu phẩy thừa ở cuối (Fix lỗi 500 hiện tại)
        options.JsonSerializerOptions.AllowTrailingCommas = true;

        // 2. Cho phép đọc số từ chuỗi (ví dụ "123" map được vào int 123)
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;

        // 3. Giữ nguyên tên property (Code cũ của bạn)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddDbContext<CleanArchitecture.Infrastructure.Data.ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// open CORS for all access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//build scope for repository and service
builder.Services.AddScoped<IProductRepository, ProductEFRepository>();
//builder.Services.AddScoped<IProductRepository, ProductDapperRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    // Ghi đè cách trả về lỗi của [ApiController]
    options.InvalidModelStateResponseFactory = context =>
    {
        // Lấy danh sách lỗi từ ModelState
        var errors = context.ModelState
                    .Where(e => e.Value!.Errors.Any())
                    .Select(e => new
                    {
                        Field = e.Key,
                        Messages = e.Value!.Errors.Select(x => x.ErrorMessage)
                    });


        return new BadRequestObjectResult(new ValidationErrorResponseDto
        {
            StatusCode = 400,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        }); ;
    };
});

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Allow All access
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();