using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(300)
            .WithMessage("Title cannot exceed 300 characters.");

        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status value.");

        RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority value.");
    }
}
