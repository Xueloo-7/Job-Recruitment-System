using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace Demo.Models;

#nullable disable warnings

public class EducationVM
{
    public string? Id { get; set; } // 系统生成，不参与验证

    [Required(ErrorMessage = "请选择用户")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "请选择学历/资质")]
    public string Qualification { get; set; }

    [Required(ErrorMessage = "请选择学校/机构")]
    public string Institution { get; set; }

    // 显示名称用
    public string? UserName { get; set; }
    //public string? QualificationName { get; set; }
    //public string? InstitutionName { get; set; }

    // 下拉列表选项
    public List<SelectListItem>? UserOptions { get; set; }
    public List<SelectListItem>? QualificationOptions { get; set; }
    public List<SelectListItem>? InstitutionOptions { get; set; }
}