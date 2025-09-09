namespace Demo.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class UserEditViewModel
{
    public string Id { get; set; }

    [Required]
    public Role Role { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class JobEditViewModel
{
    public string Id { get; set; }
    public JobStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SelectListItem>? StatusOptions { get; set; }
}

public class StatisticsViewModel
{
    // KPI 指标
    public int TotalUsers { get; set; }
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public string TopJobCategory { get; set; }
    public decimal TotalIncome { get; set; }

    // 图表数据
    public List<int> UserGrowth { get; set; } = new();
    public List<int> Applications { get; set; } = new();
    public List<decimal> Incomes { get; set; } = new();
    public List<string> Months { get; set; } = new();


    // 岗位类别分布
    public Dictionary<string, int> JobCategories { get; set; } = new();

    // TopIndustries
    public Dictionary<string, int> TopIndustries { get; set; } = new();
}