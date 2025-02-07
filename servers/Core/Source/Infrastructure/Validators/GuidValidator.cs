using System;
using FluentValidation;

namespace Core.Infrastructure.Validators;

public class GuidValidator : AbstractValidator<Guid>
{
    public GuidValidator()
    {
        RuleFor(x => x).NotEmpty().Must(x => Guid.TryParse(x.ToString(), out _));
    }
}