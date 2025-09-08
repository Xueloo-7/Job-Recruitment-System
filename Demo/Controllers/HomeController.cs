using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Demo.Controllers;

public class HomeController : BaseController
{
    private readonly DB db;
    private readonly string currentUserId = "U001"; // 模拟当前用户 ID，实际应用中应从登录状态获取

    public HomeController(DB context)
    {
        db = context;
    }

    public IActionResult Index(string keyword = "", string location = "", string? categoryId = null, string? jobPostingUserId = null)
    {
        ViewBag.Active = "Search";

        IQueryable<Job> jobQuery = db.Jobs
            .Include(j => j.Category)
            .Include(j => j.User) // 如果页面要显示发布者信息
            .Where(j => j.Status == JobStatus.Approved);

        if (!string.IsNullOrEmpty(jobPostingUserId))
        {
            // 忽略 keyword 和 location，只获取该用户发布的岗位
            jobQuery = jobQuery.Where(j => j.UserId == jobPostingUserId);
        }
        else
        {
            // 普通搜索逻辑
            if (!string.IsNullOrEmpty(keyword))
            {
                jobQuery = jobQuery.Where(j =>
                    j.Title.Contains(keyword) ||
                    j.CompanyName.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(location))
            {
                jobQuery = jobQuery.Where(j =>
                    j.Location.Contains(location));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                jobQuery = jobQuery.Where(j => j.CategoryId == categoryId);
            }
        }

        var jobs = jobQuery.ToList();

        // 获取未读通知数量
        var unreadCount = db.Notifications
            .Where(n => n.UserId == currentUserId && !n.IsRead)
            .Count();

        var vm = new HomeVM
        {
            CategoryOptions = db.Categories
                .Where(c => c.ParentId != null) // 过滤非 employer 分类
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList(),

            Jobs = jobs,

            UnreadNotificationCount = unreadCount,
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Index(HomeVM vm)
    {
        // 处理表单提交
        return RedirectToAction("Index", new
        {
            keyword = vm.Keyword,
            location = vm.Location,
            categoryId = vm.CategoryId,
            jobPostingUserId = vm.JobPostingUserId
        });
    }


    public IActionResult Employer()
    {
        // if already logged in, redirect to EmployerInfo
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("EmployerInfo", "Home");
        }
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult LoadJobs(string company)
    {
        // put database future
        var jobs = new List<string>();

        switch (company?.ToLower())
        {
            case "intel":
                jobs = new List<string> { "Software Engineer", "QA Tester", "Intern Developer" };
                break;
            case "lazada":
                jobs = new List<string> { "Frontend Developer", "UI/UX Designer", "System Analyst" };
                break;
            case "google":
                jobs = new List<string> { "AI Researcher", "Cloud Architect", "Technical Writer" };
                break;
        }

        ViewBag.Company = company;
        return PartialView("~/Views/Job/_JobListPartial.cshtml", jobs);
    }

    // EmployerInfo
    public IActionResult EmployerInfo()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = db.Users
                     .Where(u => u.Id == userId && u.Role == Role.Employer)
                     .FirstOrDefault();

        if (user == null)
            return NotFound("Employer not found.");
        if(user.Role != Role.Employer)
            return BadRequest("User is not an employer.");

        return View(user); // 传给 EmployerInfo.cshtml
    }

    // ---------------- Edit Employer ----------------

    [HttpGet]
    public IActionResult EditEmployer()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var employer = db.Users
            .FirstOrDefault(u => u.Id == userId && u.Role == Role.Employer);

        if (employer == null)
        {
            return NotFound();
        }

        var vm = new EditEmployerVM
        {
            Id = employer.Id,
            FirstName = employer.FirstName,
            LastName = employer.LastName,
            Location = employer.Location,
            Email = employer.Email,
            PhoneNumber = employer.PhoneNumber,
            IsActive = employer.IsActive
        };

        return PartialView("_EditEmployer", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditEmployer(EditEmployerVM vm)
    {
        if (!ModelState.IsValid)
        {
            DebugModelStateErrors();
            return PartialView("_EditEmployer", vm);
        }

        var employer = db.Users.FirstOrDefault(u => u.Id == vm.Id);
        if (employer == null) return NotFound();

        // 映射
        employer.FirstName = vm.FirstName;
        employer.LastName = vm.LastName;
        employer.Location = vm.Location;
        employer.Email = vm.Email;
        employer.PhoneNumber = vm.PhoneNumber;
        employer.IsActive = vm.IsActive;
        employer.UpdatedAt = DateTime.Now;

        db.Update(employer);
        db.SaveChanges();

        return PartialView("EmployerInfo", employer);
    }


    private void DebugModelStateErrors()
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }
}
