using Core.Infrastructure.Validators;
using Core.Routes.Admins.Commands;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class AssignGroupsCommandValidator : AbstractValidator<AssignGroupsCommand>
{
    public AssignGroupsCommandValidator()
    {
        RuleFor(x => x.AdminId).SetValidator(new GuidValidator());
        RuleFor(x => x.GroupIds).NotEmpty();
        RuleForEach(x => x.GroupIds).NotEmpty();
    }
}