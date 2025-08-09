namespace Demo.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class HomeVM
{
    // 下拉列表选项
    public List<SelectListItem>? CategoryOptions { get; set; }

    public List<Job> Jobs { get; set; }

    public int UnreadNotificationCount { get; set; }

    // 表单提交
    public string? Keyword { get; set; } = String.Empty;
    public string? Location { get; set; } = String.Empty;
    public string? CategoryId { get; set; } = String.Empty;
    public string? JobPostingUserId { get; set; } = String.Empty;
}