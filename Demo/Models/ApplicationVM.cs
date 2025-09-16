namespace Demo.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


public class ApplicationVM
{
    public string? Id { get; set; } // 系统生成
    public string? JobId { get; set; }
    public string? UserId { get; set; }
    [Required(ErrorMessage = "Please select a source before submitting !!!")]
    public ApplicationSource? Source { get; set; }
    [Required(ErrorMessage = "Please select a Notice Time before submitting !!!")]
    public NoticeTime? NoticeTime { get; set; }
    [Required(ErrorMessage = "Please select your salary expected before submitting !!!")]
    public SalaryExpected? SalaryExpected { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }


    public List<SelectListItem>? StatusOptions { get; set; }
    public List<SelectListItem>? JobOptions { get; set; }//下拉选项
    public List<SelectListItem>? UserOptions { get; set; }
    [ValidateNever]
    public List<SelectListItem> Sources { get; set; }
    [ValidateNever]
    public List<SelectListItem> SalaryExpecteds { get; set; }
    [ValidateNever]
    public List<SelectListItem> NoticeTimes { get; set; }


}
