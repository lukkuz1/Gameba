using DemoRest2024Live.Data;
using DemoRest2024Live.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DemoRest2024Live
{
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
                if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
                {
                    return Results.BadRequest(new ErrorResponse("Category name and description are required.", 400));
                }

                var category = new Category { Name = dto.Name, Description = dto.Description };
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Created Category ID: {category.Id}");

                return TypedResults.Created($"/api/v1/categories/{category.Id}", category.ToDto());
            }).WithName("CreateCategory");

            app.MapGet("/api/v1/categories/{id}", async (int id, GamebaDbContext dbContext) =>
            {
                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    return Results.NotFound(new ErrorResponse($"Category with ID: {id} not found.", 404));
                }

                return TypedResults.Ok(category.ToDto());
            }).WithName("GetCategory");

            app.MapPut("/api/v1/categories/{id}", async (int id, UpdateCategoryDto dto, GamebaDbContext dbContext) =>
            {
                if (dto == null || (dto.Name == null && dto.Description == null))
                {
                    return Results.UnprocessableEntity(new ErrorResponse("Either name or description must be provided.", 422));
                }

                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    return Results.NotFound(new ErrorResponse($"Category with ID: {id} not found.", 404));
                }

                category.Name = dto.Name ?? category.Name;
                category.Description = dto.Description ?? category.Description;
                await dbContext.SaveChangesAsync();

                return Results.Ok(category.ToDto());
            }).WithName("UpdateCategory");

            app.MapDelete("/api/v1/categories/{id}", async (int id, GamebaDbContext dbContext) =>
            {
                var category = await dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    return Results.NotFound(new ErrorResponse($"Category with ID: {id} not found.", 404));
                }

                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            }).WithName("DeleteCategory");
        }

        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/categories/{categoryId}/games", async (int categoryId, GamebaDbContext dbContext) =>
            {
                var games = await dbContext.Games.Where(g => g.CategoryId == categoryId).ToListAsync();
                return games;
            }).WithName("GetGamesByCategory");

            app.MapPost("/api/v1/categories/{categoryId}/games", async (int categoryId, CreateGameDto dto, GamebaDbContext dbContext) =>
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                {
                    return Results.BadRequest(new ErrorResponse("Game title and description are required.", 400));
                }

                var game = new Game { Title = dto.Title, Description = dto.Description, CategoryId = categoryId };
                dbContext.Games.Add(game);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/api/v1/categories/{categoryId}/games/{game.Id}", game);
            }).WithName("CreateGame");

            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    return Results.NotFound(new ErrorResponse($"Game with ID: {gameId} not found in Category ID: {categoryId}.", 404));
                }

                return TypedResults.Ok(game);
            }).WithName("GetGame");

            app.MapPut("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, UpdateGameDto dto, GamebaDbContext dbContext) =>
            {
                if (dto == null || (dto.Title == null && dto.Description == null))
                {
                    return Results.UnprocessableEntity(new ErrorResponse("Either title or description must be provided.", 422));
                }

                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    return Results.NotFound(new ErrorResponse($"Game with ID: {gameId} not found in Category ID: {categoryId}.", 404));
                }

                game.Title = dto.Title ?? game.Title;
                game.Description = dto.Description ?? game.Description;
                await dbContext.SaveChangesAsync();

                return Results.Ok(game);
            }).WithName("UpdateGame");

            app.MapDelete("/api/v1/categories/{categoryId}/games/{gameId}", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.CategoryId == categoryId);
                if (game == null)
                {
                    return Results.NotFound(new ErrorResponse($"Game with ID: {gameId} not found in Category ID: {categoryId}.", 404));
                }

                dbContext.Games.Remove(game);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            }).WithName("DeleteGame");
        }

        public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}/comments", async (int categoryId, int gameId, GamebaDbContext dbContext) =>
            {
                var comments = await dbContext.Comments.Where(c => c.GameId == gameId).ToListAsync();
                return comments;
            }).WithName("GetComments");

            app.MapPost("/api/v1/categories/{categoryId}/games/{gameId}/comments", async (int categoryId, int gameId, CreateCommentDto dto, GamebaDbContext dbContext) =>
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                {
                    return Results.UnprocessableEntity(new ErrorResponse("Comment content is required.", 422));
                }

                var comment = new Comment { Content = dto.Content, CreatedAt = DateTimeOffset.UtcNow, GameId = gameId };
                dbContext.Comments.Add(comment);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/api/v1/categories/{categoryId}/games/{gameId}/comments/{comment.Id}", comment);
            }).WithName("CreateComment");

            app.MapGet("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, GamebaDbContext dbContext) =>
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    return Results.NotFound(new ErrorResponse($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.", 404));
                }

                return TypedResults.Ok(comment);
            }).WithName("GetComment");

            app.MapPut("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, UpdateCommentDto dto, GamebaDbContext dbContext) =>
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                {
                    return Results.UnprocessableEntity(new ErrorResponse("Updated comment content is required.", 422));
                }

                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    return Results.NotFound(new ErrorResponse($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.", 404));
                }

                comment.Content = dto.Content;
                await dbContext.SaveChangesAsync();

                return Results.Ok(comment);
            }).WithName("UpdateComment");

            app.MapDelete("/api/v1/categories/{categoryId}/games/{gameId}/comments/{commentId}", async (int categoryId, int gameId, int commentId, GamebaDbContext dbContext) =>
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.GameId == gameId);
                if (comment == null)
                {
                    return Results.NotFound(new ErrorResponse($"Comment with ID: {commentId} not found for Game ID: {gameId} in Category ID: {categoryId}.", 404));
                }

                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            }).WithName("DeleteComment");
        }
    }
}
