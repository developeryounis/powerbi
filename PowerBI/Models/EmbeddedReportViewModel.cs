// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

using Microsoft.PowerBI.Api.Models;

namespace PowerBILab01.Models
{
    public class EmbeddedReportViewModel
    {
        // Type of the object to be embedded
        public string Type { get; set; } = "Report";

        // Report to be embedded
        public ReportViewModel Report { get; set; }

        // Embed Token for the Power BI report
        public EmbedToken Token { get; set; }

       
    }

    public class ReportViewModel
    {
        // Id of Power BI report to be embedded
        public Guid ReportId { get; set; }

        // Name of the report
        public string ReportName { get; set; }

        // Embed URL for the Power BI report
        public string EmbedUrl { get; set; }
    }
}