using FluentValidation;
using PowerBILab01.Options;

namespace PowerBILab01.Validations
{
    public class PowerBIOptionsValidator : AbstractValidator<PowerBIOptions>
    {
        private const string VALIDATION_MESSAGE = "is not set in appsettings.json file";
        public PowerBIOptionsValidator()
        {
            RuleFor(x => x.WorkspaceId.Trim())
                .NotNull()
                .NotEmpty()
                .WithMessage(m => $"{nameof(m.WorkspaceId)} {VALIDATION_MESSAGE}");

            RuleFor(x => x.WorkspaceId.Trim())
                .NotNull()
                .NotEmpty()
                .Must(c => Guid.TryParse(c, out var workspaceId))
                .WithMessage(m => $"{nameof(m.WorkspaceId)} invalid GUID");


            RuleFor(x => x.ReportId.Trim())
                .NotNull()
                .NotEmpty()
                .WithMessage(m => $"{nameof(m.ReportId)} {VALIDATION_MESSAGE}");

            RuleFor(x => x.ReportId.Trim())
                .NotNull()
                .NotEmpty()
                .Must(c => Guid.TryParse(c, out var reportId))
                .WithMessage(m => $"{nameof(m.ReportId)} invalid GUID");
        }
    }
}
