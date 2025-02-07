
using Core.Infrastructure.Validators;
using Core.Routes.Admins.Queries;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class GetAdminByIdQueryValidator : AbstractValidator<GetAdminByIdQuery>
{
    public GetAdminByIdQueryValidator()
    {
        RuleFor(x => x.AdminId).NotEmpty().SetValidator(new GuidValidator());
    }
}