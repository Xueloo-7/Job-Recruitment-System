namespace Demo.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings



public class JobClassifyVM
{
    public string? JobId { get; set; } // Null for new jobs, set for editing existing jobs
    public Guid? DraftId { get; set; }
    [Required, MaxLength(100)]
    public string Title { get; set; }
    [Required, MaxLength(100)]
    public string Location { get; set; }
    [Required]
    public WorkType? WorkType { get; set; }
    [Required]
    public string? CategoryId { get; set; }
    [Required]
    public PayType? PayType { get; set; }
    [Range(0, 99999)]
    public decimal SalaryMin { get; set; }
    [Range(0, 99999)]
    public decimal SalaryMax { get; set; }

    // Dropdown lists
    public List<SelectListItem> WorkTypeOptions { get; set; } = new();
    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> PayTypeOptions { get; set; } = new();
}

public class JobSubscriptionVM
{
    public string? JobId { get; set; } // Null for new jobs, set for editing existing jobs
    public Guid? DraftId { get; set; }
    public string PromotionId { get; set; }
    public List<Promotion> Promotions { get; set; } = new();
}

public class JobWriteVM
{
    public string? JobId { get; set; } // Null for new jobs, set for editing existing jobs
    public Guid? DraftId { get; set; }
    [Required, MaxLength(2000)]
    public string Description { get; set; }
    [Required, MaxLength(200)]
    public string Summary { get; set; }
    public IFormFile? Logo { get; set; }          // 用来接收上传的文件
    public string? LogoImageUrl { get; set; }     // 用来显示预览（保存后的路径）
}

public class JobPaymentVM
{
    public Guid? DraftId { get; set; }
    public Promotion? Promotion { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}

public class JobCandidatesVM
{
    public string JobId { get; set; }
    public string JobTitle { get; set; }
    public List<Application> Candidates { get; set; } = new();
}

public class CandidateDetailVM
{
    public string ApplicationId { get; set; }
    public string JobTitle { get; set; }

    // User 基本信息
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Location { get; set; }
    public string Summary { get; set; }

    // 简历
    public string? ResumeUrl { get; set; }

    // 申请信息
    public DateTime AppliedAt { get; set; }
    public ApplicationStatus Status { get; set; }
    public string ExpectedSalary { get; set; }
    public NoticeTime NoticeTime { get; set; }

    // Career History
    public List<JobExperience> CareerHistory { get; set; } = new();

    // Education
    public List<Education> EducationHistory { get; set; } = new();
}

