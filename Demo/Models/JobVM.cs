namespace Demo.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class JobVM
{
    public string? Id { get; set; } // 系统生成，不参与验证

    [Required]
    public string UserId { get; set; }

    [Required]  // 【FK】
    public string CategoryId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    [Required]
    public PayType PayType { get; set; }

    [Required]
    public WorkType WorkType { get; set; }

    [Required]
    public decimal SalaryMin { get; set; }

    [Required]
    public decimal SalaryMax { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Summary { get; set; }

    [MaxLength(255)]
    public string? LogoImageUrl { get; set; }

    public bool IsOpen { get; set; } = true;

    public IFormFile? LogoFile { get; set; } // ✅ 新增：用来接收文件上传

    // ✅ 下拉列表绑定字段
    public List<SelectListItem>? UserOptions { get; set; }
    public List<SelectListItem>? CategoryOptions { get; set; }
    public List<SelectListItem>? PayTypeOptions { get; set; }
    public List<SelectListItem>? WorkTypeOptions { get; set; }

}