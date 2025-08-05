namespace Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class ApplicationVM
{
    public string? Id { get; set; } // 系统生成
    public string JobId { get; set; }
    public string UserId { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }

    public List<SelectListItem>? JobOptions { get; set; }
    public List<SelectListItem>? UserOptions { get; set; }
    public List<SelectListItem>? StatusOptions { get; set; }
}
