namespace Demo.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class JobVM
{
    public string? Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string CategoryId { get; set; }

    [Required]
    public string PromotionId { get; set; }  // ✅ 新增 Promotion 选择字段

    [Required(ErrorMessage = "Title cannot be empty")]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required(ErrorMessage = "Company Name cannot be empty")]
    [MaxLength(30)]
    public string CompanyName { get; set; }

    
    [Required, MaxLength(100)]
    public string Location { get; set; }


    [Required(ErrorMessage = "Please select a Pay Type")]
    public PayType PayType { get; set; }

    [Required(ErrorMessage = "Please select a Work Type")]
    public WorkType WorkType { get; set; }

    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99999.99")]
    [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Salary format")]
    public decimal SalaryMin { get; set; }

    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99999.99")]
    [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Salary format")]
    public decimal SalaryMax { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Please select a photo.")]
    [MaxLength(255)]
    public IFormFile LogoImageUrl { get; set; }

    public bool IsOpen { get; set; } = true;

    public IFormFile? LogoFile { get; set; }

    // ✅ 下拉选项
    public List<SelectListItem>? UserOptions { get; set; }
    public List<SelectListItem>? CategoryOptions { get; set; }
    public List<SelectListItem>? PayTypeOptions { get; set; }
    public List<SelectListItem>? WorkTypeOptions { get; set; }
    public List<SelectListItem>? PromotionOptions { get; set; } // ✅ 新增
}
