using DemoRest2024Live.Data;
using DemoRest2024Live.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Text.Json;

namespace DemoRest2024Live
{
    public static class Endpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/categories", async (GamebaDbContext dbContext) =>
            {
                var categories = await dbContext.Categories.ToListAsync();
                var categoryDtos = categories.Select(c => c.ToDto()).ToList();
                Console.WriteLine($"Fetched Categories: {JsonSerializer.Serialize(categoryDtos)}");
                return categoryDtos;
            }).WithName("GetCategories");

            app.MapPost("/api/v1/categories", async (CreateCategoryDto dto, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Creating Category: {JsonSerializer.Serialize(dto)}");

                // Create new category instance from DTO
                var category = new Category { Name = dto.Name, Description = dto.Description };
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Created Category ID: {category.Id}");

                return TypedResults.Created($"/api/v1/categories/{category.Id}", category.ToDto());
            }).WithName("CreateCategory");

            app.MapGet("/api/v1/categories/{id}", async (int id, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Fetching Category with ID: {id}");

                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    Console.WriteLine($"Category with ID: {id} not found.");
                    return Results.NotFound();
                }

                Console.WriteLine($"Fetched Category: {JsonSerializer.Serialize(category.ToDto())}");
                return TypedResults.Ok(category.ToDto());
            }).WithName("GetCategory");

            app.MapPut("/api/v1/categories/{id}", async (int id, UpdateCategoryDto dto, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Updating Category with ID: {id} using data: {JsonSerializer.Serialize(dto)}");

                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    Console.WriteLine($"Category with ID: {id} not found for update.");
                    return Results.NotFound();
                }

                // Update category properties if provided
                if (dto.Name != null) category.Name = dto.Name;
                if (dto.Description != null) category.Description = dto.Description;

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Updated Category: {JsonSerializer.Serialize(category.ToDto())}");
                return Results.Ok(category.ToDto());
            }).WithName("UpdateCategory");

            app.MapDelete("/api/v1/categories/{id}", async (int id, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Deleting Category with ID: {id}");

                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    Console.WriteLine($"Category with ID: {id} not found for deletion.");
                    return Results.NotFound();
                }

                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Deleted Category ID: {id}");
                return Results.NoContent();
            }).WithName("DeleteCategory");
        }

        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/categories/{categoryId}/games", async (int categoryId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Fetching Games for Category ID: {categoryId}");
                var games = await dbContext.Games.Where(g => g.CategoryId == categoryId).ToListAsync();
                Console.WriteLine($"Fetched Games: {JsonSerializer.Serialize(games)}");
                return games;
            }).WithName("GetGamesByCategory");

            app.MapPost("/api/v1/categories/{categoryId}/games", async (int categoryId, CreateGameDto dto, GamebaDbContext dbContext) =>
{
    // Logging the incoming request
    Console.WriteLine($"Creating Game in Category ID: {categoryId} with data: {JsonSerializer.Serialize(dto)}");

    // Input validation
    if (dto == null)
    {
        return Results.BadRequest("Game data is required.");
    }

    if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
    {
        return Results.BadRequest("Game title and description are required.");
    }

    // Creating a new game object
    var game = new Game
    {
        Title = dto.Title,
        Description = dto.Description,
        CategoryId = categoryId
    };

    // Adding game to the database
    dbContext.Games.Add(game);

    try
    {
        // Save changes to the database
        await dbContext.SaveChangesAsync();
        Console.WriteLine($"Created Game ID: {game.Id}");

        // Returning a success response with the created game
        return Results.Created($"/api/v1/categories/{categoryId}/games/{game.Id}", game);
    }
    catch (Exception ex)
    {
        // Logging the error
        Console.WriteLine($"Error creating game: {ex.Message}");
        return Results.StatusCode(500);
    }
}).WithName("CreateGame");

            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Fetching Game with ID: {gameId} in Category ID: {categoryId}");
                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    Console.WriteLine($"Game with ID: {gameId} not found in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                Console.WriteLine($"Fetched Game: {JsonSerializer.Serialize(game)}");
                return TypedResults.Ok(game);
            }).WithName("GetGame");

            app.MapPut("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, UpdateGameDto dto, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Updating Game with ID: {gameId} in Category ID: {categoryId} using data: {JsonSerializer.Serialize(dto)}");

                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    Console.WriteLine($"Game with ID: {gameId} not found in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                game.Title = dto.Title ?? game.Title;
                game.Description = dto.Description ?? game.Description;
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Updated Game: {JsonSerializer.Serialize(game)}");
                return Results.Ok(game);
            }).WithName("UpdateGame");

            app.MapDelete("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Deleting Game with ID: {gameId} in Category ID: {categoryId}");

                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    Console.WriteLine($"Game with ID: {gameId} not found in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                dbContext.Games.Remove(game);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Deleted Game ID: {gameId} in Category ID: {categoryId}");
                return Results.NoContent();
            }).WithName("DeleteGame");
        }

        public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}/comments", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Fetching Comments for Game ID: {gameId} in Category ID: {categoryId}");
                var comments = await dbContext.Comments.Where(c => c.GameId == gameId).ToListAsync();
                Console.WriteLine($"Fetched Comments: {JsonSerializer.Serialize(comments)}");
                return comments;
            }).WithName("GetComments");

            app.MapPost("/api/v1/categories/{categoryId}/games/{gameId}/comments", async (int categoryId, int gameId, CreateCommentDto dto, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Creating Comment for Game ID: {gameId} in Category ID: {categoryId} with data: {JsonSerializer.Serialize(dto)}");

                var comment = new Comment { Content = dto.Content, CreatedAt = DateTimeOffset.UtcNow, GameId = gameId };
                dbContext.Comments.Add(comment);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Created Comment ID: {comment.Id}");

                return TypedResults.Created($"/api/v1/categories/{categoryId}/games/{gameId}/comments/{comment.Id}", comment);
            }).WithName("CreateComment");

            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Fetching Comment with ID: {commentId} for Game ID: {gameId} in Category ID: {categoryId}");
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    Console.WriteLine($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                Console.WriteLine($"Fetched Comment: {JsonSerializer.Serialize(comment)}");
                return TypedResults.Ok(comment);
            }).WithName("GetComment");

            app.MapPut("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, UpdateCommentDto dto, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Updating Comment with ID: {commentId} for Game ID: {gameId} in Category ID: {categoryId} using data: {JsonSerializer.Serialize(dto)}");

                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    Console.WriteLine($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                comment.Content = dto.Content;
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Updated Comment: {JsonSerializer.Serialize(comment)}");
                return Results.Ok(comment);
            }).WithName("UpdateComment");

            app.MapDelete("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, GamebaDbContext dbContext) =>
            {
                Console.WriteLine($"Deleting Comment with ID: {commentId} for Game ID: {gameId} in Category ID: {categoryId}");

                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    Console.WriteLine($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.");
                    return Results.NotFound();
                }

                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Deleted Comment ID: {commentId} for Game ID: {gameId} in Category ID: {categoryId}");
                return Results.NoContent();
            }).WithName("DeleteComment");
        }
    }
}
