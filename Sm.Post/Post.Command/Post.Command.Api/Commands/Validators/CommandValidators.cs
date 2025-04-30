using FluentValidation;
using Post.Command.Api.DTO;

namespace Post.Command.Api.Commands.Validators;

public class NewPostDtoValidator : AbstractValidator<NewPostDto>
{
    public NewPostDtoValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500);
    }
}

public class EditMessageDtoValidator : AbstractValidator<EditMessageDto>
{
    public EditMessageDtoValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500);
    }
}