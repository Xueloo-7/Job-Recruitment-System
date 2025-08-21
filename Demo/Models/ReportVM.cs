using Microsoft.AspNetCore.Mvc;

namespace Demo.Models;
#nullable disable warnings

public class ReportVM
{
    public string UserId { get; set; } = string.Empty;
    public List<JobReportVM> JobReports { get; set; } = new List<JobReportVM>();
}

public class JobReportVM
{
    public Job Job { get; set; }
    public int TotalApplications { get; set; }
    public int TotalHired { get; set; }
    public int RecruitmentDays { get; set; }
    public bool IsOpen { get; set; }
}