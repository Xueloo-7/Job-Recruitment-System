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
            Status = ApplicationStatus.Offered,
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
            Status = ApplicationStatus.Offered,
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
                    TotalHired = _db.Applications.Count(a => a.JobId == j.Id && a.Status == ApplicationStatus.Offered),
                    RecruitmentDays = (DateTime.Now - j.CreatedAt).Days,
                    Status = j.Status
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

        //Get All applications for the employer within the date range
        List<Application> applications = _db.Applications
            .Include(a => a.Job)
            .Where(a => a.Job.UserId == userId && a.CreatedAt >= start && a.CreatedAt <= end)
            .ToList();

        if (!applications.Any())
            return Json(new { message = "No application data in this date range" });

        // Pie Data
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

        // Line Data
        var totalDays = (end - start).TotalDays;
        List<string> lineLabels;
        List<int> lineData;

        if (totalDays <= 14) // Time range <= 14 day → day data
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
        else // time range is big → week data
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
