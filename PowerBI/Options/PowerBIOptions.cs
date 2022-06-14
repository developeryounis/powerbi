// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBILab01.Options
{
    public class PowerBIOptions
    {
        // Workspace Id for which Embed token needs to be generated
        public string WorkspaceId { get; set; }

        // Report Id for which Embed token needs to be generated
        public string ReportId { get; set; }
    }
}