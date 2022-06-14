// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

using PowerBILab01.Models;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using PowerBILab01.Options;
using Microsoft.PowerBI.Api.Models.Credentials;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;

namespace PowerBILab01.Services
{
    public interface IPowerBIEmbeddedService
    {
        Task<EmbeddedReportViewModel> GetEmbeddedReportAsync(Guid? reportId);
        Task<IEnumerable<ReportViewModel>> GetListOfReportsInWorkspaceAsync();
    }
    public class PowerBIEmbeddedService : IPowerBIEmbeddedService
    {
        private readonly ReportMappingOptions _reportMappingOptions = new ReportMappingOptions();
        private const string POWERBI_API_URL = "https://api.powerbi.com";
        private readonly IPowerBIClient _powerBIClient;
        private readonly PowerBIOptions _powerBIOptions;
        private readonly Guid _workspaceId;

        public PowerBIEmbeddedService(IAzureADAccessService azureADAccessService,
            IOptions<List<ReportOptions>> reportMappingOptions,
            IOptions<PowerBIOptions> powerBIOptions)
        {
            _reportMappingOptions.Reports = reportMappingOptions.Value;
            _powerBIClient = new PowerBIClient(new Uri(POWERBI_API_URL), azureADAccessService.GetToken());
            _powerBIOptions = powerBIOptions.Value;
            _workspaceId = new Guid(_powerBIOptions.WorkspaceId);
        }

        public async Task<IEnumerable<ReportViewModel>> GetListOfReportsInWorkspaceAsync()
        {
            var workspaceId = new Guid(_powerBIOptions.WorkspaceId);
            List<ReportViewModel> reports = new List<ReportViewModel>();
            // Get report info
            var pbiReports = await _powerBIClient.Reports.GetReportsInGroupAsync(workspaceId);

            return pbiReports.Value.Select(rpt => new ReportViewModel()
            {
                ReportId = rpt.Id,
                ReportName = rpt.Name,
                EmbedUrl = rpt.EmbedUrl
            });

        }

        public async Task<EmbeddedReportViewModel> GetEmbeddedReportAsync(Guid? reportId)
        {
            var newReportId = reportId ?? new Guid(_powerBIOptions.ReportId);
            // Get report info
            var pbiReport = await _powerBIClient.Reports.GetReportInGroupAsync(_workspaceId, newReportId);

            //  Check if dataset is present for the corresponding report
            //  If isRDLReport is true then it is a RDL Report 
            var isRDLReport = string.IsNullOrEmpty(pbiReport.DatasetId) && pbiReport.ReportType != "PaginatedReport";

            EmbedToken embedToken;

            // Generate embed token for RDL report if dataset is not present
            if (isRDLReport)
            {
                // Get Embed token for RDL Report
                embedToken = await GetEmbedTokenForRDLReportAsync(_workspaceId, newReportId);
            }
            else
            {
                // Create list of datasets
                var datasetIds = new List<Guid>();
                if (!string.IsNullOrEmpty(pbiReport.DatasetId))
                {

                    // Add dataset associated to the report
                    datasetIds.Add(Guid.Parse(pbiReport.DatasetId));
                }

                // Get Embed token multiple resources
                embedToken = await GetEmbedTokenAsync(newReportId, datasetIds, _workspaceId);
            }

            return new EmbeddedReportViewModel
            {
                Report = new ReportViewModel
                {
                    ReportId = pbiReport.Id,
                    ReportName = pbiReport.Name,
                    EmbedUrl = pbiReport.EmbedUrl
                },
                Token = embedToken
            };
        }

        private void UpdateDatasourceCredentials(Guid targetWorkspaceId, Guid reportId, List<DatasourceCredentialOptions> creds)
        {
            //_powerBIClient.Reports.get
            var dataSources = _powerBIClient.Reports.GetDatasources(targetWorkspaceId, reportId);

            foreach (var dsCred in creds)
            {
                var ds = dataSources.Value.Single(ds => ds.DatasourceId == dsCred.DatasourceId);
                if (ds.DatasourceType == "Sql" && ds.GatewayId != null)
                {
                    var basicCred = new BasicCredentials(dsCred.Username, dsCred.Password);
                    var credDetails = new CredentialDetails(basicCred, PrivacyLevel.Organizational, EncryptedConnection.Encrypted);
                    var request = new UpdateDatasourceRequest(credDetails);
                    _powerBIClient.Gateways.UpdateDatasource(ds.GatewayId.Value, ds.DatasourceId!.Value, request);
                }
            }
        }

        /// <summary>
        /// Get Embed token for single report, multiple datasets, and an optional target workspace
        /// </summary>
        /// <returns>Embed token</returns>
        /// <remarks>This function is not supported for RDL Report</remakrs>
        private async Task<EmbedToken> GetEmbedTokenAsync(Guid reportId, List<Guid> datasetIds, [Optional] Guid targetWorkspaceId)
        {
            // Create a request for getting Embed token 
            // This method works only with new Power BI V2 workspace experience
            var reportOptions = _reportMappingOptions[reportId];
            if (reportOptions != null && reportOptions.DatasetIDs.Count > 0)
            {
                datasetIds.AddRange(reportOptions.DatasetIDs);
                datasetIds = datasetIds.Distinct().ToList();
            }
            if (reportOptions != null && reportOptions.DatasourceCredentials.Count > 0)
            {
                UpdateDatasourceCredentials(targetWorkspaceId, reportId, reportOptions?.DatasourceCredentials);
            }

            var tokenRequest = new GenerateTokenRequestV2(

                reports: new List<GenerateTokenRequestV2Report>() { new GenerateTokenRequestV2Report(reportId) },

                datasets: datasetIds?.Select(datasetId => new GenerateTokenRequestV2Dataset(datasetId.ToString(), XmlaPermissions.ReadOnly)).ToList(),

                targetWorkspaces: targetWorkspaceId != Guid.Empty ? new List<GenerateTokenRequestV2TargetWorkspace>() { new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId) } : null
            );

            // Generate Embed token
            var embedToken = await _powerBIClient.EmbedToken.GenerateTokenAsync(tokenRequest);

            return embedToken;
        }

        /// <summary>
        /// Get Embed token for RDL Report
        /// </summary>
        /// <returns>Embed token</returns>
        private async Task<EmbedToken> GetEmbedTokenForRDLReportAsync(Guid targetWorkspaceId, Guid reportId, string accessLevel = "view")
        {
            // Generate token request for RDL Report
            var generateTokenRequestParameters = new GenerateTokenRequest(
                accessLevel: accessLevel
            );


            //V2 https://github.com/Microsoft/PowerBI-CSharp
            // Generate Embed token
            var embedToken = await _powerBIClient.Reports.GenerateTokenInGroupAsync(targetWorkspaceId, reportId, generateTokenRequestParameters);


            return embedToken;
        }
    }
}