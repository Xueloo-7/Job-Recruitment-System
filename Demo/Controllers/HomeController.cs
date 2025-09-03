using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            .Where(j => j.IsOpen);

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
    public IActionResult EmployerInfo(string? userId)
    {
        var user = db.Users
                     .Where(u => u.Id == userId && u.Role == Role.Employer)
                     .FirstOrDefault();

        if (user == null)
        {
            return NotFound("Employer not found.");
        }

        return View(user); // 传给 EmployerInfo.cshtml
    }

    // ---------------- Edit Employer ----------------

    [HttpGet]
    public IActionResult EditEmployer(string id)
    {
        var employer = db.Users
            .Include(u => u.Jobs)
            .Include(u => u.Applications)
            .FirstOrDefault(u => u.Id == id && u.Role == Role.Employer);

        if (employer == null)
        {
            return NotFound();
        }

        return PartialView("_EditEmployer", employer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditEmployer(User model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditEmployer", model);
        }

        var employer = db.Users.FirstOrDefault(u => u.Id == model.Id);
        if (employer == null)
        {
            return NotFound();
        }

        // 更新
        employer.FirstName = model.FirstName;
        employer.LastName = model.LastName;
        employer.Location = model.Location;
        employer.Email = model.Email;
        employer.PhoneNumber = model.PhoneNumber;
        employer.IsActive = model.IsActive;
        employer.UpdatedAt = DateTime.Now;

        db.Update(employer);
        db.SaveChanges();

        return RedirectToAction("EmployerInfo", new { userId = employer.Id });
    }
}
