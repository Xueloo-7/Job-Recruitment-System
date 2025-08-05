namespace Demo.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Key, MaxLength(6)]
    [RegularExpression(@"^U\d{3}$", ErrorMessage = "ID 格式应为 U+三位数字")]
    [Remote(action: "CheckUserId", controller: "Test", ErrorMessage = "ID 已存在")]
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
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public bool HasExperience { get; set; } = false;

    public ICollection<Education>? Educations { get; set; }

    public ICollection<JobExperience>? JobExperiences { get; set; }

    public Resume? Resume { get; set; }

    public ICollection<Job>? Jobs { get; set; }

    public ICollection<Application>? Applications { get; set; }

    public ICollection<Notification>? Notifications { get; set; }
}

public enum Role
{
    Admin,
    JobSeeker,
    Employer
}

public class Education
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^E\d{3}$", ErrorMessage = "ID 格式应为 E+三位数字")]
    [Remote(action: "CheckEducationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required, MaxLength(50)]
    public string Qualification { get; set; }

    [Required, MaxLength(100)]
    public string Institution { get; set; }

    // 【导航属性】
    public User User { get; set; }
}

public class JobExperience
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^JE\d{3}$", ErrorMessage = "ID 格式应为 JE+三位数字")]
    [Remote(action: "CheckJobExperienceId", controller: "Test", ErrorMessage = "ID 已存在")]
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
    [Key, MaxLength(6)]
    [RegularExpression(@"^R\d{3}$", ErrorMessage = "ID 格式应为 R+三位数字")]
    [Remote(action: "CheckResumeId", controller: "Test", ErrorMessage = "ID 已存在")]
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
    [Key, MaxLength(6)]
    [RegularExpression(@"^Q\d{3}$", ErrorMessage = "ID 格式应为 Q+三位数字")]
    [Remote(action: "CheckQualificationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }
}

public class Institution
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^I\d{3}$", ErrorMessage = "ID 格式应为 I+三位数字")]
    [Remote(action: "CheckInstitutionId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }
}

public class Category
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^C\d{3}$", ErrorMessage = "ID 格式应为 C+三位数字")]
    [Remote(action: "CheckCategoryId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public string? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public Category? Parent { get; set; }

    public ICollection<Category>? Children { get; set; }
}

public class Job
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^J\d{3}$", ErrorMessage = "ID 格式应为 J+三位数字")]
    [Remote(action: "CheckJobId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required]  // 【FK】
    public string CategoryId { get; set; }

    [Required]  // 【FK】
    public string PromotionId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    [Required]
    public PayType PayType { get; set; }

    [Required]
    public WorkType WorkType { get; set; }

    [Required]
    public decimal SalaryMin { get; set; }

    [Required]
    public decimal SalaryMax { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Summary { get; set; }

    [MaxLength(255)]
    public string? LogoImageUrl { get; set; }

    public bool IsOpen { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public User User { get; set; }
    public Category Category { get; set; }
    public Promotion Promotion { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
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
    [Key, MaxLength(6)]
    [RegularExpression(@"^A\d{3}$", ErrorMessage = "ID 格式应为 A+三位数字")]
    [Remote(action: "CheckApplicationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string JobId { get; set; }

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
    [Key, MaxLength(6)]
    [RegularExpression(@"^N\d{3}$", ErrorMessage = "ID 格式应为 N+三位数字")]
    [Remote(action: "CheckNotificationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

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
    [Key, MaxLength(6)]
    [RegularExpression(@"^P\d{3}$", ErrorMessage = "ID 格式应为 P+三位数字")]
    [Remote(action: "CheckPromotionId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int DurationDay { get; set; }

    public string Description { get; set; }

    // 修改导航属性：从 JobPromotion → Job（1对多）
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
