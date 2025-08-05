namespace Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class ResumeVM
{
    public string? Id { get; set; } // 系统生成，不参与验证

    [Required]
    public string UserId { get; set; }

    public string? ImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public List<SelectListItem> UserOptions { get; set; } = new();
}
