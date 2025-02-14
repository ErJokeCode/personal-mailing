using Core.Infrastructure.Validators;
using Core.Routes.Documents.Commands;
using FluentValidation;

namespace Core.Routes.Documents.Validators;

public class DownloadDocumentCommandValidator : AbstractValidator<DownloadDocumentCommand>
{
    public DownloadDocumentCommandValidator()
    {
        RuleFor(x => x.BlobId).SetValidator(new GuidValidator());
    }
}

