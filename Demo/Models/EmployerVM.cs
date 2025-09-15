namespace Demo.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class JobDashboardVM
{
    public string JobId { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public int Candidates { get; set; }   // 应聘人数
    public int Hired { get; set; }        // 已录取人数
}

public class EmployerDashboardVM
{
    public string EmployerName { get; set; }
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int TotalHires { get; set; }
    public List<JobDashboardVM> Jobs { get; set; }
}