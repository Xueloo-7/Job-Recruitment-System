namespace Demo.Models;

using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class InfoViewModel
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";
    [Required, Phone, MaxLength(20)]
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
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public int StartMonth { get; set; }
    public int StartYear { get; set; }
    public int EndMonth { get; set; }
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