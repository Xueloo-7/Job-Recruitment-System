namespace Demo.Models;

using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class InfoViewModel
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";
    [Required]
    [RegularExpression(@"^\+?\d{7,20}$", ErrorMessage = "Invalid phone number.")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = "";
    [MaxLength(100)] public string? Location { get; set; }
    [MaxLength(100)] public string? FirstName { get; set; }
    [MaxLength(100)] public string? LastName { get; set; }
}
public class SummaryViewModel
{
    [MaxLength(1000)]
    public string? Summary { get; set; }
}

public class CareerPageViewModel
{
    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
    public string CompanyName { get; set; }

    [Range(1, 12, ErrorMessage = "Start month must be between 1 and 12.")]
    public int StartMonth { get; set; }

    [Range(1900, 2100, ErrorMessage = "Start year must be a valid year.")]
    public int StartYear { get; set; }

    [Range(1, 12, ErrorMessage = "End month must be between 1 and 12.")]
    public int EndMonth { get; set; }

    [Range(1900, 2100, ErrorMessage = "End year must be a valid year.")]
    public int EndYear { get; set; }

    public bool StillInRole { get; set; }

    public IEnumerable<JobExperience> ExistingJobExperiences { get; set; } = new List<JobExperience>();
}


public class EducationPageViewModel
{
    public string Qualification { get; set; }
    public string Institution { get; set; }
    public IEnumerable<Education> ExistingEducations { get; set; } = new List<Education>();
}