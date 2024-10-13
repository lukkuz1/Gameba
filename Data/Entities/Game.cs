using System.Collections.Generic;

namespace DemoRest2024Live.Data.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        // Convert Game entity to GameDto
        public GameDto ToDto()
        {
            return new GameDto
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                CategoryId = this.CategoryId
            };
        }

        // Create a Game entity from GameDto
        public static Game FromDto(GameDto dto)
        {
            return new Game
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };
        }
    }

    // DTO Class for Game
    public class GameDto
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }
    }

    // DTO Class for Creating a Game
    public class CreateGameDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; } // Reference to the Category this game belongs to
    }

    // DTO Class for Updating a Game
    public class UpdateGameDto
    {
        public string? Title { get; set; } // Optional update for title
        public string? Description { get; set; } // Optional update for description
        public int? CategoryId { get; set; } // Optional update for category
    }
}
