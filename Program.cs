using System.Text.Json;
using DemoRest2024Live;
using DemoRest2024Live.Data;
using DemoRest2024Live.Validators; // Add this line
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models; // Add this line for Swagger

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<GamebaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use PostgreSQL connection

// Add FluentValidation automatic validation for the endpoints
builder.Services.AddFluentValidationAutoValidation(configuration => { });
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>(); // Register validators
builder.Services.AddResponseCaching();
builder.Services.AddEndpointsApiExplorer(); // Add support for endpoint exploration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoRest API", Version = "v1" }); // Swagger setup
}); // Add Swagger generator
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // Optional: Configure JSON options
});

var app = builder.Build();

// API root endpoint
app.MapGet("/api", (HttpContext httpContext, LinkGenerator linkGenerator) => Results.Ok(new List<LinkDto>
{
    new(linkGenerator.GetUriByName(httpContext, "GetCategories"), "categories", "GET"),
})).WithName("GetRoot");

// Error Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (FluentValidation.ValidationException validationEx) // Handle FluentValidation exceptions
    {
        var errorResponse = new ErrorResponse(validationEx.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation failed.", 400);
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
    catch (Exception)
    {
        var errorResponse = new ErrorResponse("An unexpected error occurred.", 400);
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoRest API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Map API Endpoints
app.MapCategoryEndpoints();
app.MapGameEndpoints();
app.MapCommentEndpoints();

// Middleware
app.UseResponseCaching();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class LinkDto
{
    public string Href { get; }
    public string Rel { get; }
    public string Method { get; }

    public LinkDto(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}

// ErrorResponse class for structured error messages
public class ErrorResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }

    public ErrorResponse(string message, int statusCode)
    {
        Message = message;
        StatusCode = statusCode;
        Timestamp = DateTime.UtcNow;
    }
}
