using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class ReportController : Controller
{
    private readonly DB _db;

    private User demoUser = new User
    {
        Id = "U000",
        FirstName = "Demo",
        LastName = "User",
        Email = "",
        Role = Role.Employer,  
    };

    private List<Application> demoApplications = new List<Application>
    {
        new Application
        {
            Id = "A001",
            UserId = "U000",
            Status = ApplicationStatus.Hired,
            CreatedAt = DateTime.Now.AddDays(-10),
            UpdatedAt = DateTime.Now,
            Source = ApplicationSource.LinkedIn,
        },
        new Application
        {
            Id = "A002",
            UserId = "U000",
            Status = ApplicationStatus.Rejected,
            CreatedAt = DateTime.Now.AddDays(-5),
            UpdatedAt = DateTime.Now,
            Source = ApplicationSource.Indeed,
        },
        new Application
        {
            Id = "A003",
            UserId = "U000",
            Status = ApplicationStatus.Hired,
            CreatedAt = DateTime.Now.AddDays(-3),
            UpdatedAt = DateTime.Now,
            Source = ApplicationSource.Unknown,
        }
    };

    public ReportController(DB context)
    {
        _db = context;
    }

    public IActionResult Index(string? userId = "")
    {
        var user = _db.Users.Find(userId);

        if (user == null)
            user = demoUser;

        if (user.Role != Role.Employer)
            return Unauthorized();

        var vm = new ReportVM
        {
            UserId = user.Id,

            JobReports = _db.Jobs
                .Where(j => j.UserId == user.Id)
                .Select(j => new JobReportVM
                {
                    Job = j,
                    TotalApplications = _db.Applications.Count(a => a.JobId == j.Id),
                    TotalHired = _db.Applications.Count(a => a.JobId == j.Id && a.Status == ApplicationStatus.Hired),
                    RecruitmentDays = (DateTime.Now - j.CreatedAt).Days,
                    IsOpen = j.IsOpen
                })
                .ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult GetChartData(string userId, DateTime start, DateTime end)
    {
        var user = _db.Users.Find(userId);

        if (user == null)
            return NotFound("User not found");

        if(user.Role != Role.Employer)
            return Unauthorized("Only employers can access this data");

        List<Application> applications = _db.Applications
            .Include(a => a.Job)
            .Where(a => a.Job.UserId == userId && a.CreatedAt >= start && a.CreatedAt <= end)
            .ToList();

        Debug.WriteLine($"Found {applications.Count} applications for user {userId} between {start} and {end}");

        if (!applications.Any())
            return Json(new { message = "No application data in this date range" });

        // 饼图数据（来源统计）
        var sourceCounts = new Dictionary<ApplicationSource, int>();
        foreach (var app in applications)
        {
            if (sourceCounts.TryGetValue(app.Source, out int count))
                sourceCounts[app.Source] = count + 1;
            else
                sourceCounts[app.Source] = 1;
        }
        var pieLabels = Enum.GetNames(typeof(ApplicationSource));
        var pieData = Enum.GetValues(typeof(ApplicationSource))
            .Cast<ApplicationSource>()
            .Select(source => sourceCounts.TryGetValue(source, out int count) ? count : 0)
            .ToArray();

        // 折线图数据（按天或周聚合）
        var totalDays = (end - start).TotalDays;
        List<string> lineLabels;
        List<int> lineData;

        if (totalDays <= 14) // 时间跨度 <= 14 天 → 按天
        {
            lineLabels = Enumerable.Range(0, (int)totalDays + 1)
                .Select(i => start.AddDays(i).ToString("MM-dd"))
                .ToList();

            lineData = lineLabels.Select(label =>
            {
                var date = DateTime.ParseExact(label, "MM-dd", null);
                return applications.Count(a => a.CreatedAt.Date == date.Date);
            }).ToList();
        }
        else // 时间跨度大 → 按周
        {
            var startWeek = start.Date;
            var endWeek = end.Date;
            var weekCount = (int)Math.Ceiling((endWeek - startWeek).TotalDays / 7);

            lineLabels = Enumerable.Range(0, weekCount)
                .Select(i => $"第{i + 1}周")
                .ToList();

            lineData = Enumerable.Range(0, weekCount)
                .Select(i =>
                {
                    var weekStart = startWeek.AddDays(i * 7);
                    var weekEnd = weekStart.AddDays(6);
                    return applications.Count(a => a.CreatedAt.Date >= weekStart && a.CreatedAt.Date <= weekEnd);
                }).ToList();
        }

        var data = new
        {
            pieLabels,
            pieData,
            lineLabels,
            lineData
        };
        return Json(data);
    }
}
