using System;

namespace DemoRest2024Live.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public required string Content { get; set; }

        public required DateTimeOffset CreatedAt { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }

        // Convert Comment entity to CommentDto
        public CommentDto ToDto()
        {
            return new CommentDto
            {
                Id = this.Id,
                Content = this.Content,
                CreatedAt = this.CreatedAt,
                GameId = this.GameId
            };
        }

        // Create a Comment entity from CommentDto
        public static Comment FromDto(CommentDto dto)
        {
            return new Comment
            {
                Id = dto.Id,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt,
                GameId = dto.GameId
            };
        }
    }

    // DTO Class for Comment
    public class CommentDto
    {
        public int Id { get; set; }

        public required string Content { get; set; }

        public required DateTimeOffset CreatedAt { get; set; }

        public int GameId { get; set; }
    }

    // DTO Class for Creating a Comment
    public class CreateCommentDto
    {
        public required string Content { get; set; }
        public int GameId { get; set; } // Reference to the Game this comment belongs to
    }

    // DTO Class for Updating a Comment
    public class UpdateCommentDto
    {
        public string? Content { get; set; } // Optional update for content
    }
}
