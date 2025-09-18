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
    public DbSet<JobDraft> JobDrafts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Qualification> Qualifications { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
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

public class User : IHasId
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

    public string? Summary { get; set; }

    [Required]
    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(100)]
    public string Location { get; set; } = "";

    public bool HasExperience { get; set; } = false;
    public string CompanyName { get; set; } = "None";

    public ICollection<Education>? Educations { get; set; }

    public ICollection<JobExperience>? JobExperiences { get; set; }

    public ICollection<Resume>? Resumes { get; set; }

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

public class Education : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^E\d{3}$", ErrorMessage = "ID 格式应为 E+三位数字")]
    [Remote(action: "CheckEducationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    [Required, MaxLength(100)]
    public string Qualification { get; set; }

    [Required, MaxLength(100)]
    public string Institution { get; set; }

    // 【导航属性】
    public User User { get; set; }
}

public class JobExperience : IHasId
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

public class Resume : IHasId
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

public class Qualification : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^Q\d{3}$", ErrorMessage = "ID 格式应为 Q+三位数字")]
    [Remote(action: "CheckQualificationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }
}

public class Institution : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^I\d{3}$", ErrorMessage = "ID 格式应为 I+三位数字")]
    [Remote(action: "CheckInstitutionId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }
}

public class Category : IHasId
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

public class Job : IHasId
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

    [Required]
    public string Location { get; set; }

    [Required]
    public PayType PayType { get; set; }

    [Required]
    public WorkType WorkType { get; set; }

    [Required]
    public decimal SalaryMin { get; set; }

    [Required]
    public decimal SalaryMax { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Summary { get; set; }

    [MaxLength(255)]
    public string? LogoImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public JobStatus Status { get; set; } = JobStatus.Pending;

    // 【导航属性】
    public User User { get; set; }
    public Category Category { get; set; }
    public Promotion Promotion { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

public enum JobStatus
{
    Approved, // 已审核通过，会被显示
    Rejected, // 已审核拒绝，不会被显示
    Pending, // 待审核,
    Withdrawn
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

public class JobDraft
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? JobId { get; set; }  // 关联已提交的 Job（如果有）

    [Required, MaxLength(6)]  // FK
    public string UserId { get; set; }

    // 和 Job 相似，但大部分字段允许为空（因为可能没填完）
    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(30)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public PayType? PayType { get; set; }
    public WorkType? WorkType { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Summary { get; set; }

    [MaxLength(255)]
    public string? LogoImageUrl { get; set; }

    public string? CategoryId { get; set; }
    public string? PromotionId { get; set; }

    public int LastStep { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public User User { get; set; }
}


public class Application : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^A\d{3}$", ErrorMessage = "ID 格式应为 A+三位数字")]
    [Remote(action: "CheckApplicationId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string JobId { get; set; }

    [Required, MaxLength(6)]  // 【FK】
    public string UserId { get; set; }

    public ApplicationSource Source { get; set; } = ApplicationSource.Unknown; // 申请来源

    public DateTime? HiredDate { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    [Required]
    public SalaryExpected SalaryExpected { get; set; }
    [Required]
    public NoticeTime NoticeTime { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    // 【导航属性】
    public Job Job { get; set; }
    public User User { get; set; }
    public Resume? Resume { get; set; }  // 可选的简历
}

public enum ApplicationStatus
{
    Pending,
    Interview,
    Offered,
    Rejected
}

public enum ApplicationSource
{
    JobSeeker, // 求职者主动申请
    LinkedIn, // LinkedIn
    GoogleAds, // 谷歌广告
    Indeed, // 招聘网站
    JobFair, // 招聘会
    Referral, // 内推
    Unknown // 未知来源
}

public enum SalaryExpected
{
    [Display(Name = "Not need to pay for me.")]
    RM0_1,
    [Display(Name = "RM 1K")]
    RM1K,
    [Display(Name = "RM 1.5K")]
    RM1_5K,
    [Display(Name = "RM 2K")]
    RM2K,
    [Display(Name = "RM 2.5K")]
    RM2_5K,
    [Display(Name = "RM 3K")]
    RM3K,
    [Display(Name = "RM 3.5K")]
    RM3_5K,
    [Display(Name = "RM 4K")]
    RM4K,
    [Display(Name = "RM 4.5K")]
    RM4_5K,
    [Display(Name = "RM 5K")]
    RM5K,
    [Display(Name = "RM 5.5K")]
    RM5_5K,
    [Display(Name = "RM 6K")]
    RM6K,
    [Display(Name = "RM 7K")]
    RM7K,
    [Display(Name = "RM 8K")]
    RM8K,
    [Display(Name = "RM 10K")]
    RM10K,
    [Display(Name = "RM 12K")]
    RM12K,
    [Display(Name = "RM 15K")]
    RM15K,
    [Display(Name = "RM 20K")]
    RM20K,
    [Display(Name = "RM 30K")]
    RM30K,
    [Display(Name = "RM 40K")]
    RM40K,
    [Display(Name = "RM50K or more")]
    RM50KOrMore
}

public enum NoticeTime
{
    [Display(Name = "None, I'm ready to go now")]
    NoneImReadyToGoNow,
    [Display(Name = "Less than 2 weeks")]
    LessThanTwoWeek,
    [Display(Name = "1 month")]
    OneMonths,
    [Display(Name = "2 months")]
    TwoMonths,
    [Display(Name = "3 months")]
    ThreeMonths,
    [Display(Name = "More than 3 months")]
    MoreThanThreeMonths
}

public enum NotificationType
{
    Application,
    Interview,
    System,
    Reminder,
    Account
}

public enum NotificationChannel
{
    InApp
}

public class Notification : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^N\d{3}$", ErrorMessage = "ID 格式应为 N+三位数字")]
    public string Id { get; set; }

    [MaxLength(6)]
    public string UserId { get; set; }   // 接收者 ID

    [MaxLength(6)]
    public string? FromUserId { get; set; } // 触发人（如 Jobseeker）

    [Required, MaxLength(100)]
    public string Title { get; set; }

    public string Content { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NotificationType Type { get; set; }

    public string? RelatedEntityId { get; set; } // 例如 JobId 或 ApplicationId

    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

    // 导航属性
    public User User { get; set; }
    public User FromUser { get; set; }
}

public class Promotion : IHasId
{
    [Key, MaxLength(6)]
    [RegularExpression(@"^P\d{3}$", ErrorMessage = "ID 格式应为 P+三位数字")]
    [Remote(action: "CheckPromotionId", controller: "Test", ErrorMessage = "ID 已存在")]
    public string Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public int DurationDay { get; set; }

    public string Description { get; set; }

    // 修改导航属性：从 JobPromotion → Job（1对多）
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(6)]  // 接收者 ID
    public string UserId { get; set; }

    [Required, MaxLength(100)]
    public string TableName { get; set; }  // 被操作的数据表，比如 Job

    [Required, MaxLength(50)]
    public string ActionType { get; set; }  // 操作类型: Create, Update, Delete

    [MaxLength(100)]
    public string RecordId { get; set; }  // 被操作记录的主键，比如 Job.Id

    public string? Changes { get; set; }  // 保存变更详情（JSON 格式比较常见）

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // 导航属性
    public User User { get; set; }
}