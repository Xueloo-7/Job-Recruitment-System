namespace Demo.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable warnings

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<JobExperience> JobExperiences { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Qualification> Qualifications { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<JobPromotion> JobPromotions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // decimal 精度配置
        modelBuilder.Entity<Job>()
            .Property(j => j.SalaryMin)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Job>()
            .Property(j => j.SalaryMax)
            .HasPrecision(10, 2);

        // Application.UserId 禁用级联
        modelBuilder.Entity<Application>()
            .HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification.UserId 禁用级联
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}

public class User
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; }

    [Required, MaxLength(100)]
    public string PasswordHash { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    public bool HasExperience { get; set; }

    // 【导航属性】
    public ICollection<Education> Educations { get; set; } = new List<Education>();
    public ICollection<JobExperience> JobExperiences { get; set; } = new List<JobExperience>();
    public Resume Resume { get; set; }
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public enum Role
{
    Admin,
    JobSeeker,
    Employer
}

public class Education
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required, MaxLength(50)]
    public string Qualification { get; set; }

    [Required, MaxLength(50)]
    public string Institution { get; set; }

    // 【导航属性】
    public User User { get; set; }
}

public class JobExperience
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required, MaxLength(50)]
    public string JobTitle { get; set; }

    [Required, MaxLength(50)]
    public string CompanyName { get; set; }

    [Required]
    public int StartYear { get; set; }

    [Required]
    public int StartMonth { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }

    [Required]
    public bool StillInRole { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public User User { get; set; }
}

public class Resume
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public User User { get; set; }
}

public class Qualification
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }
}

public class Institution
{
    [Key, Required, MaxLength(6)]  // 【PK】
    public string Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }
}

public class Category
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public int? ParentId { get; set; }  // 【FK】
}

public class Job
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required]  // 【FK】
    public int CategoryId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    [Required]
    public PayType PayType { get; set; }

    [Required]
    public WorkType WorkType { get; set; }

    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }

    public string Description { get; set; }

    [MaxLength(500)]
    public string Summary { get; set; }

    [MaxLength(255)]
    public string LogoImageUrl { get; set; }

    public bool IsOpen { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public User User { get; set; }
    public Category Category { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<JobPromotion> JobPromotions { get; set; } = new List<JobPromotion>();
}

public enum PayType
{
    Hourly,
    Monthly,
    Annual
}

public enum WorkType
{
    FullTime,
    PartTime,
    Contract,
    Casual
}

public class Application
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required]  // 【FK】
    public int JobId { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public Job Job { get; set; }
    public User User { get; set; }
}

public enum ApplicationStatus
{
    Pending,
    Interview,
    Hired,
    Rejected
}

public class Notification
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    public string Content { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public User User { get; set; }
}

public class Promotion
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int DurationDay { get; set; }

    public string Description { get; set; }

    // 【导航属性】
    public ICollection<JobPromotion> JobPromotions { get; set; } = new List<JobPromotion>();
}

public class JobPromotion
{
    [Key]  // 【PK】
    public int Id { get; set; }

    [Required]  // 【FK】
    public int JobId { get; set; }

    [Required]  // 【FK】
    public int PromotionId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public Job Job { get; set; }
    public Promotion Promotion { get; set; }
}
