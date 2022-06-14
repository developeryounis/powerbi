using FluentValidation;
using PowerBILab01.Options;

namespace PowerBILab01.Validations
{
    public class AzureAdOptionsValidator : AbstractValidator<AzureAdOptions>
    {
        private const string VALIDATION_MESSAGE = "is not set in appsettings.json file";
        public AzureAdOptionsValidator()
        {
            RuleFor(x => x.AuthorityUri.Trim())
                .NotNull()
                .NotEmpty()
                .WithMessage(m => $"{nameof(m.AuthorityUri)} {VALIDATION_MESSAGE}");

            RuleFor(x => x.ClientId.Trim())
                .NotNull()
                .NotEmpty()
                .WithMessage(m => $"{nameof(m.ClientId)} {VALIDATION_MESSAGE}");

            RuleFor(x => x.TenantId.Trim())
                .NotNull()
                .NotEmpty()
                .WithMessage(m => $"{nameof(m.TenantId)} {VALIDATION_MESSAGE}");


            RuleFor(x => x)
                .Must(m => !string.IsNullOrWhiteSpace(m.ClientId) && !string.IsNullOrWhiteSpace(m.ClientSecret))
                .When(c => !string.IsNullOrWhiteSpace(c.AuthenticationMode) && c.AuthenticationMode.ToLower() == "serviceprincipal")
                .WithMessage("Service Principal Authenticator required ClientId and ClientSecret");

            RuleFor(x => x)
                .Must(m => !string.IsNullOrWhiteSpace(m.PbiUsername) && !string.IsNullOrWhiteSpace(m.PbiPassword))
                .When(c => !string.IsNullOrWhiteSpace(c.AuthenticationMode) && c.AuthenticationMode.ToLower() == "masteruser")
                .WithMessage("Master User Authenticator required PbiUsername and PbiPassword");


            RuleFor(x => x.Scope)
                .NotNull()
                .Must(c => c.Length > 0)
                .WithMessage(m => $"{nameof(m.Scope)} {VALIDATION_MESSAGE}");


           
        }
    }
}
