using DemoRest2024Live.Data.Entities;
using FluentValidation;

namespace DemoRest2024Live.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Category name is required.");
        }
    }

    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(c => c.Name).NotEmpty().When(c => !string.IsNullOrWhiteSpace(c.Description)).WithMessage("Either name or description must be provided.");
        }
    }

    public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
    {
        public CreateGameDtoValidator()
        {
            RuleFor(g => g.Title).NotEmpty().WithMessage("Game title is required.");
        }
    }

    public class UpdateGameDtoValidator : AbstractValidator<UpdateGameDto>
    {
        public UpdateGameDtoValidator()
        {
            RuleFor(g => g.Title).NotEmpty().When(g => string.IsNullOrWhiteSpace(g.Description)).WithMessage("Either title or description must be provided.");
        }
    }

    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(c => c.Content).NotEmpty().WithMessage("Comment content is required.");
        }
    }

    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(c => c.Content).NotEmpty().WithMessage("Updated comment content is required.");
        }
    }
}
