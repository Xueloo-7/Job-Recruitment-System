namespace Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class SearchViewModel
{
    public string Placeholder { get; set; } = "请输入搜索内容";
    public string Action { get; set; } // 提交到的 Action
    public string Controller { get; set; } // 提交到的 Controller
    public string Query { get; set; } // 搜索关键字
    public string Method { get; set; } = "GET"; // GET 或 POST
}

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

public class EditEmployerVM
{
    public string Id { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; set; }

    public bool IsActive { get; set; }
}

public class ApplyPageVM  // 组合 ApplicationVM 和 ResumeVM
{
    public ApplicationVM Application { get; set; } = new ApplicationVM();
    public ResumeVM Resume { get; set; } = new ResumeVM();
}

public class ApplicationListVM
{
    public string ApplicationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime? HiredDate { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
