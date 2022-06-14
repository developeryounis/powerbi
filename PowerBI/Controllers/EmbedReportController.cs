using Microsoft.AspNetCore.Mvc;
using PowerBILab01.Services;
using PowerBILab01.Models;
using FluentValidation.Results;

namespace PowerBILab01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbedReportController : ControllerBase
    {
        private readonly IPowerBIEmbeddedService _powerBIEmbeddedService;
        private readonly ValidationResult _configValidationResult;

        public EmbedReportController(IPowerBIEmbeddedService powerBIEmbeddedService,
            IConfigValidator configValidatorService)
        {
            _powerBIEmbeddedService = powerBIEmbeddedService;
            _configValidationResult = configValidatorService.ValidateConfig();
        }

        [HttpGet("{reportId?}")]
        public async Task<IActionResult> Get(Guid? reportId)
        {
            try
            {
                if (!_configValidationResult.IsValid)
                {
                    return NotFound(_configValidationResult.Errors);
                }

                EmbeddedReportViewModel embedParams = await _powerBIEmbeddedService.GetEmbeddedReportAsync(reportId);

                return Ok(embedParams);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportList()
        {

            try
            {

                if (!_configValidationResult.IsValid)
                {
                    return NotFound(_configValidationResult.Errors);
                }

                var reports = await _powerBIEmbeddedService.GetListOfReportsInWorkspaceAsync();

                return Ok(reports);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
