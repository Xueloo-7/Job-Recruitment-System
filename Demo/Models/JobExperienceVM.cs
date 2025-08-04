namespace Demo.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
#nullable disable warnings

public class JobExperienceVM
{
    public string? Id { get; set; } // 系统生成，不参与验证

    [Required]
    public string UserId { get; set; }

    [Required, MaxLength(50)]
    public string JobTitle { get; set; }

    [Required, MaxLength(50)]
    public string CompanyName { get; set; }

    [Required]
    public int StartYear { get; set; }

    [Required]
    public int StartMonth { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }

    [Required]
    public bool StillInRole { get; set; }

    // ✅ 下拉列表绑定字段
    public List<SelectListItem>? UserOptions { get; set; }
    public List<SelectListItem>? YearOptions { get; set; }
    public List<SelectListItem>? MonthOptions { get; set; }

    public string? UserName { get; set; } // 用户名，用于显示
}
