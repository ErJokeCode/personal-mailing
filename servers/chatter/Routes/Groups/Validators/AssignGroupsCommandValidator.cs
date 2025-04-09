using Chatter.Routes.Groups.Commands;
using FluentValidation;

namespace Chatter.Routes.Admins.Validators;

public class AssignGroupsCommandValidator : AbstractValidator<AssignGroupsCommand>
{
    public AssignGroupsCommandValidator()
    {
        RuleFor(x => x.GroupIds)
            .NotEmpty()
            .WithName("Группы");
    }
}