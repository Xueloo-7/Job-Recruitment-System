namespace Demo.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings



public class JobClassifyVM
{
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
    public Guid? DraftId { get; set; }
    public string PromotionId { get; set; }
    public List<Promotion> Promotions { get; set; } = new();
}

public class JobWriteVM
{
    public Guid? DraftId { get; set; }
    [Required, MaxLength(2000)]
    public string Description { get; set; }
    [Required, MaxLength(200)]
    public string Summary { get; set; } 
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