using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(AuthenticationSchemes = "AdminCookie", Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DB _db;
    private readonly Helper hp;

    public AdminController(DB _context, Helper _hp)
    {
        this._db = _context;
        this.hp = _hp;
    }

    #region Authentication
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginVM vm, string? returnURL)
    {
        var user = _db.Users.Where(u => u.Email == vm.Email).FirstOrDefault();

        if (user == null || !hp.VerifyPassword(user.PasswordHash, vm.Password))
        {
            ModelState.AddModelError("", "Invalid credentials");
        }
        else if (!user.IsActive)
        {
            ModelState.AddModelError("", "Account is inactive. Please contact administrator.");
        }
        else if (user.Role != Role.Admin)
        {
            ModelState.AddModelError("", "Access denied. Admins only.");
        }
        else if (ModelState.IsValid)
        {
            // 登录成功
            TempData["Info"] = "Login Successfully.";
            hp.SignIn(user, vm.RememberMe);

            if (string.IsNullOrEmpty(returnURL))
                return RedirectToAction("Dashboard", "Admin");
            else
                return Redirect(returnURL);
        }

        ModelState.DebugErrors();

        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [AllowAnonymous]
    public IActionResult Register(RegisterVM vm)
    {
        if (ModelState.IsValid("Email") &&
            _db.Users.Any(u => u.Email == vm.Email))
        {
            ModelState.AddModelError("Email", "Duplicated Email.");
        }

        if (ModelState.IsValid)
        {
            // Insert member
            // TODO
            _db.Users.Add(new User
            {
                Id = GenerateUserId(),
                Name = GenerateUsername(vm.Email),
                PasswordHash = hp.HashPassword(vm.Password),
                Email = vm.Email,
                PhoneNumber = "",
                Role = Role.Admin,
            });

            _db.SaveChanges();

            TempData["Info"] = "Register successfully. Please login.";
            return RedirectToAction("Login");
        }
        else
        {
            ModelState.DebugErrors();
        }

        return View(vm);
    }

    private string GenerateUserId()
    {
        // 查询当前已有的最大编号
        var lastUser = _db.Users
            .Where(u => u.Id.StartsWith("U"))
            .OrderByDescending(u => u.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastUser != null)
        {
            string lastNumberStr = lastUser.Id.Substring(1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"U{nextNumber.ToString("D3")}";  // 例如 JOB001、JOB002
    }

    private string GenerateUsername(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
            return "";

        int atIndex = userEmail.IndexOf('@');
        if (atIndex > 0)
            return userEmail.Substring(0, atIndex);

        return userEmail; // 如果没有@就原样返回
    }

    public IActionResult Logout(string? returnURL)
    {
        // if not authenticated, redirect to home page
        if (!User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        TempData["Info"] = "Logout Successfully.";

        hp.AdminSignOut();

        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Dashboard
    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        return View();
    }
    #endregion

    #region User Management

    public IActionResult Users(string query)
    {
        IQueryable<User> users = _db.Users;

        // match Name, FirstName, LastName, Email
        if (!string.IsNullOrWhiteSpace(query))
        {
            users = users.Where(u =>
                u.Name.Contains(query) ||
                (u.FirstName != null && u.FirstName.Contains(query)) ||
                (u.LastName != null && u.LastName.Contains(query)) ||
                u.Email.Contains(query)
            );
        }

        return View(users.ToList());
    }

    public IActionResult UserEdit(string id)
    {
        if (id == null) return NotFound();

        var user = _db.Users.Find(id);
        if (user == null) return NotFound();

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            Role = user.Role,
            IsActive = user.IsActive,
            UpdatedAt = user.UpdatedAt
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UserEdit(UserEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.DebugErrors();
            return View(vm);
        }

        var user = _db.Users.Find(vm.Id);
        if (user == null) return NotFound();

        var changes = new List<string>();

        if (user.Role != vm.Role)
            changes.Add($"Role: '{user.Role}' -> '{vm.Role}'");

        if (user.IsActive != vm.IsActive)
            changes.Add($"IsActive: '{user.IsActive}' -> '{vm.IsActive}'");

        user.Role = vm.Role;
        user.IsActive = vm.IsActive;
        user.UpdatedAt = DateTime.Now;

        _db.SaveChanges();

        // audit log
        var log = new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Users",
            ActionType = "Update",
            RecordId = user.Id,
            Changes = string.Join(", ", changes)
        };
        _db.AuditLogs.Add(log);
        _db.SaveChanges();

        TempData["Info"] = "user updated";
        return RedirectToAction("UserEdit", new { id = vm.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UserDelete(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        _db.SaveChanges();

        // audit log
        var log = new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Users",
            ActionType = "Delete",
            RecordId = user.Id,
            Changes = $""
        };
        _db.AuditLogs.Add(log);
        _db.SaveChanges();

        TempData["Info"] = "user deleted";
        return RedirectToAction("Users"); // 返回用户列表
    }

    #endregion

    #region Job Management

    public IActionResult Jobs(string query)
    {
        IQueryable<Job> jobs = _db.Jobs
            .Include(j => j.User);

        // match Title, CompanyName
        if (!string.IsNullOrWhiteSpace(query))
        {
            jobs = jobs.Where(u => u.Title.Contains(query) || u.CompanyName.Contains(query));
        }

        return View(jobs.ToList());
    }

    public IActionResult JobEdit(string id)
    {
        if (id == null) return NotFound();

        var jobs = _db.Jobs.Find(id);
        if (jobs == null) return NotFound();

        var vm = new JobEditViewModel
        {
            Id = jobs.Id,
            Status = jobs.Status,
            UpdatedAt = jobs.UpdatedAt,
            StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = JobStatus.Pending.ToString(), Text = "Pending" },
                new SelectListItem { Value = JobStatus.Approved.ToString(), Text = "Approved" },
                new SelectListItem { Value = JobStatus.Rejected.ToString(), Text = "Rejected" },
            }
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult JobEdit(JobEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.DebugErrors();
            return View(vm);
        }

        var job = _db.Jobs.Find(vm.Id);
        if (job == null) return NotFound();

        JobStatus oldStatus = job.Status;
        job.Status = vm.Status;
        job.UpdatedAt = DateTime.Now;

        _db.SaveChanges();

        // audit log
        var log = new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Jobs",
            ActionType = "Update",
            RecordId = job.Id,
            Changes = $"Status: {oldStatus} -> {job.Status}"
        };
        _db.AuditLogs.Add(log);
        _db.SaveChanges();

        TempData["Info"] = "Job updated";
        return RedirectToAction("JobEdit", new { id = vm.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult JobDelete(string id)
    {
        var job = _db.Jobs.Find(id);
        if (job == null) return NotFound();

        _db.Jobs.Remove(job);
        _db.SaveChanges();

        // audit log
        var log = new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Jobs",
            ActionType = "Delete",
            RecordId = job.Id,
            Changes = $""
        };
        _db.AuditLogs.Add(log);
        _db.SaveChanges();

        TempData["Info"] = "Job deleted";
        return RedirectToAction("Jobs"); // 返回用户列表
    }

    #endregion

    #region Audit Logs
    public IActionResult AuditLogs(string query)
    {
        IQueryable<AuditLog> auditLogs = _db.AuditLogs.Include(a => a.User).OrderByDescending(a => a.Timestamp);

        // match TableName, ActionType, RecordId, Changes, User.Name
        if (!string.IsNullOrWhiteSpace(query))
        {
            auditLogs = auditLogs.Where(u =>
                (
                    (u.TableName ?? "") + " " +
                    (u.ActionType ?? "") + " " +
                    (u.RecordId ?? "") + " " +
                    (u.Changes ?? "") + " " +
                    (u.User.Name ?? "") + " " +
                    (u.User.FirstName ?? "") + " " +
                    (u.User.LastName ?? "")
                ).Contains(query)
            );
        }

        return View(auditLogs.ToList());
    }
    #endregion

    #region Statistics
    public IActionResult Statistics(DateTime? startDate, DateTime? endDate)
    {
        // 默认时间范围：最近6个月
        var end = endDate ?? DateTime.Now;
        var start = startDate ?? end.AddMonths(-5);

        // 动态生成月份列表
        var months = Enumerable.Range(0, ((end.Year - start.Year) * 12 + end.Month - start.Month) + 1)
            .Select(i => start.AddMonths(i).ToString("yyyy-MM"))
            .ToList();

        // Top Industries 真实数据
        var topIndustries = _db.Jobs
            .GroupBy(j => j.CompanyName)   // 按公司名分组
            .Select(g => new
            {
                Industry = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)   // 前10
            .ToDictionary(x => x.Industry, x => x.Count);

        var model = new StatisticsViewModel
        {
            TotalUsers = _db.Users.Count(),   // 预设
            TotalJobs = _db.Jobs.Count(),
            TotalApplications = _db.Applications.Count(),
            TopJobCategory = "IT",
            TotalIncome = 120000,

            UserGrowth = new List<int> { 2000, 4000, 7000, 10000, 12450 },
            Applications = new List<int> { 800, 1200, 1500, 2000, 2300 },
            Incomes = new List<decimal> { 20000, 25000, 30000, 22000, 23000 },
            Months = months,  // ✅ 动态月份

            JobCategories = new Dictionary<string, int>
            {
                { "IT/开发", 45 },
                { "市场", 20 },
                { "设计", 15 },
                { "运营", 20 }
            },

            // 使用真实统计
            TopIndustries = topIndustries
        };

        return View(model);
    }
    #endregion

    #region Job Approval

    // 待审核岗位列表
    public IActionResult JobApprovals(string query)
    {
        IQueryable<Job> jobs = _db.Jobs
            .Include(j => j.User)
            .Where(j => j.Status == JobStatus.Pending); // 只取待审核

        if (!string.IsNullOrWhiteSpace(query))
        {
            jobs = jobs.Where(u => u.Title.Contains(query) || u.CompanyName.Contains(query));
        }

        return View(jobs.ToList());
    }

    // 详情页（只读）
    public IActionResult JobApprovalDetail(string id)
    {
        if (id == null) return NotFound();

        var job = _db.Jobs
            .Include(j => j.User)
            .FirstOrDefault(j => j.Id == id && j.Status == JobStatus.Pending);

        if (job == null) return NotFound();

        return View(job);
    }

    // 批准
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApproveJob(string id)
    {
        var job = _db.Jobs.Find(id);
        if (job == null) return NotFound();

        job.Status = JobStatus.Approved;
        job.UpdatedAt = DateTime.Now;
        _db.SaveChanges();

        // 审计日志
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Jobs",
            ActionType = "Approve",
            RecordId = job.Id,
            Changes = "Job approved"
        });
        _db.SaveChanges();

        // 通知招聘者
        _db.Notifications.Add(new Notification
        {
            Id = Helper.GenerateId(_db.Notifications, "N", 3),
            UserId = job.UserId,
            FromUserId = User.GetUserId(),
            Title = "岗位审核通过",
            Content = $"您的岗位「{job.Title}」已通过审核，可以正式发布。",
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        TempData["Info"] = "Job approved.";
        return RedirectToAction("JobApprovals");
    }

    // 驳回
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RejectJob(string id, string reason)
    {
        var job = _db.Jobs.Find(id);
        if (job == null) return NotFound();

        job.Status = JobStatus.Rejected;
        job.UpdatedAt = DateTime.Now;
        _db.SaveChanges();

        // 审计日志
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = User.GetUserId(),
            TableName = "Jobs",
            ActionType = "Reject",
            RecordId = job.Id,
            Changes = $"Job rejected. Reason: {reason}"
        });
        _db.SaveChanges();

        // 通知招聘者
        _db.Notifications.Add(new Notification
        {
            Id = Helper.GenerateId(_db.Notifications, "N", 3),
            UserId = job.UserId,
            FromUserId = User.GetUserId(),
            Title = "岗位审核未通过",
            Content = $"您的岗位「{job.Title}」未通过审核。\n理由：{reason}",
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        TempData["Info"] = "Job rejected.";
        return RedirectToAction("JobApprovals");
    }

    #endregion




}
