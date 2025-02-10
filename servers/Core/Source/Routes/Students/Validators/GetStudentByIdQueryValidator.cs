using Core.Infrastructure.Validators;
using Core.Routes.Students.Queries;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class GetStudentByIdQueryValidator : AbstractValidator<GetStudentByIdQuery>
{
    public GetStudentByIdQueryValidator()
    {
        RuleFor(x => x.StudentId).SetValidator(new GuidValidator());
    }
}