namespace PowerBILab01.Options
{
    public class ReportMappingOptions
    {
        public List<ReportOptions> Reports { get; set; } = new List<ReportOptions>();
        public ReportOptions this[Guid reportId] => Reports.SingleOrDefault(x => x.ReportId == reportId);
    }

    public class DatasourceCredentialOptions
    {
        public Guid DatasourceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ReportOptions
    {
        public Guid ReportId { get; set; }
        public List<Guid> DatasetIDs { get; set; } = new List<Guid>();
        public List<DatasourceCredentialOptions> DatasourceCredentials { get; set; } = new List<DatasourceCredentialOptions>();
    }
}
