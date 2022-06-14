// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

using FluentValidation.Results;
using Microsoft.Extensions.Options;
using PowerBILab01.Options;
using PowerBILab01.Validations;

namespace PowerBILab01.Services
{
    public interface IConfigValidator
    {
        ValidationResult ValidateConfig();
    }
    public class ConfigValidator : IConfigValidator
    {
        private readonly ValidationResult _validationResult;
        public ConfigValidator(IOptions<AzureAdOptions> azureAdOptions, IOptions<PowerBIOptions> powerBIOptions)
        {
            var azureAdValidationResult = new AzureAdOptionsValidator().Validate(azureAdOptions.Value).Errors ?? new List<ValidationFailure>();
            var powerBIValidationResult = new PowerBIOptionsValidator().Validate(powerBIOptions.Value).Errors ?? new List<ValidationFailure>();

            var errors = new List<ValidationFailure>();
            errors.AddRange(azureAdValidationResult);
            errors.AddRange(powerBIValidationResult);

            _validationResult = new ValidationResult(errors);

        }
        /// <summary>
        /// Validates whether all the configuration parameters are set in appsettings.json file
        /// </summary>
        /// <param name="appSettings">Contains appsettings.json configuration values</param>
        /// <returns></returns>
        public ValidationResult ValidateConfig()
        {
            return _validationResult;
        }

    }
}