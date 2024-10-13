using System.Collections.Generic;

namespace DemoRest2024Live.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public CategoryDto ToDto()
        {
            return new CategoryDto
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description
            };
        }

        public static Category FromDto(CategoryDto dto)
        {
            return new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }
    }

    // DTO Class for Category
    public class CategoryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }

    // DTO Class for Creating a Category
    public class CreateCategoryDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; } // Optional description for category
    }

    // DTO Class for Updating a Category
    public class UpdateCategoryDto
    {
        public string? Name { get; set; } // Optional update for name
        public string? Description { get; set; } // Optional update for description
    }
}
