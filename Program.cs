using System.Text.Json;
using DemoRest2024Live;
using DemoRest2024Live.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<GamebaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use PostgreSQL connection
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration => { });
builder.Services.AddResponseCaching();

var app = builder.Build();

/*
    API Endpoints:
    Categories:
    /api/v1/categories          GET    List all categories (200)
    /api/v1/categories          POST   Create a new category (201)
    /api/v1/categories/{id}     GET    Retrieve a specific category by ID (200)
    /api/v1/categories/{id}     PUT    Update a specific category by ID (200)
    /api/v1/categories/{id}     DELETE Remove a specific category by ID (204)
    Games:
    /api/v1/categories/{id}/games         GET    List all games (200)
    /api/v1/categories/{id}/games       POST   Create a new game (201)
    /api/v1/categories/{id}/games/{gameId}    GET    Retrieve a specific game by ID (200)
    /api/v1/categories/{id}/games/{gameId}      PUT    Update a specific game by ID (200)
    /api/v1/categories/{id}/games/{gameId}     DELETE Remove a specific game by ID (204)
    Comments:
    /api/v1/categories/{id}/games/{gameId}/comments       GET    List all comments for a specific game (200)
    /api/v1/categories/{id}/games/{gameId}/comments       POST   Create a new comment for a specific game (201)
    /api/v1/categories/{id}/games/{gameId}/comments/{commentId} GET    Retrieve a specific comment by category and game ids (200)
    /api/v1/categories/{id}/games/{gameId}/comments/{commentId} PUT    Update a specific comment by ID (200)
    /api/v1/categories/{id}/games/{gameId}/comments/{commentId} DELETE Remove a specific comment by ID (204)
*/

// API root endpoint
app.MapGet("/api", (HttpContext httpContext, LinkGenerator linkGenerator) => Results.Ok(new List<LinkDto>
{
    new(linkGenerator.GetUriByName(httpContext, "GetCategories"), "categories", "GET"),
})).WithName("GetRoot");

// Map API Endpoints
app.MapCategoryEndpoints();
app.MapGameEndpoints();
app.MapCommentEndpoints();

// Middleware
app.UseResponseCaching();
app.UseHttpsRedirection();
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