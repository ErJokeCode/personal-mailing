using Core.Routes.Groups.Commands;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class AssignGroupsCommandValidator : AbstractValidator<AssignGroupsCommand>
{
    public AssignGroupsCommandValidator()
    {
        RuleFor(x => x.GroupIds).NotEmpty();
        RuleForEach(x => x.GroupIds).NotEmpty();
    }
}