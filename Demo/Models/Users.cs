using System.ComponentModel.DataAnnotations;

namespace Demo.Models;

#nullable disable warnings

public class User
{
    [Key, Required, MaxLength(6)]
    public string Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }

    [Required, MaxLength(100)]
    public string PasswordHash { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    public bool HasExperience { get; set; }

    public ICollection<Education> Educations { get; set; }

    public ICollection<JobExperience> JobExperiences { get; set; }

    public Resume Resume { get; set; }

    public ICollection<Job> Jobs { get; set; }

    public ICollection<Application> Applications { get; set; }

    public ICollection<Notification> Notifications { get; set; }
}
