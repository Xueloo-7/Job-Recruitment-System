using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class TestController : Controller
{
    private readonly DB _db;

    public TestController(DB context)
    {
        _db = context;
    }

    // 首页：查看所有表数据总览（数据条数）
    public IActionResult Index()
    {
        var model = new TestDashboardViewModel
        {
            UserCount = _db.Users.Count(),
            JobCount = _db.Jobs.Count(),
            ApplicationCount = _db.Applications.Count(),
            CategoryCount = _db.Categories.Count(),
            QualificationCount = _db.Qualifications.Count(),
            InstitutionCount = _db.Institutions.Count(),
            NotificationCount = _db.Notifications.Count(),
            EducationCount = _db.Educations.Count(),
            JobExperienceCount = _db.JobExperiences.Count()
            // 你可以继续添加其它表的数据量
        };

        return View(model);
    }

    // User =============================================================================================================== User
    #region User
    #region GET
    public IActionResult Users()
    {
        var users = _db.Users.ToList();
        return View("User/Index", users);
    }

    public IActionResult CreateUser()
    {
        return View("User/Create");
    }

    public IActionResult EditUser(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return View("User/Edit", user);
    }

    public IActionResult DeleteUser(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return View("User/Delete", user);
    }

    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateUser(UserVM vm)
    {
        // Model验证
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Id = GenerateUserId(),
                Name = GenerateUsername(vm.Email),
                PasswordHash = vm.PasswordHash,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Role = vm.Role
            };

            _db.Users.Add(user);
            _db.SaveChanges();
        }
        else
        {
            DebugModelStateErrors();
        }

        return RedirectToAction("Users");
    }

    [HttpPost]
    public IActionResult EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
        return View("User/Edit", user);
    }

    [HttpPost, ActionName("DeleteUser")]
    public IActionResult DeleteUserConfirmed(string id)
    {
        var user = _db.Users.Find(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
        }
        return RedirectToAction("Users");
    }

    #endregion

    #region Functions
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
            string lastNumberStr = lastUser.Id.Substring(3);
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

    public IActionResult CheckUserId(string id)
    {
        bool exists = _db.Users.Any(u => u.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }

    #endregion
    #endregion

    // Category =============================================================================================================== Category
    #region Category
    #region GET
    public IActionResult Categories()
    {
        var categories = _db.Categories.ToList();

        var categoryVMs = categories.Select(c => new CategoryVM
        {
            Id = c.Id,
            Name = c.Name,
            ParentName = c.ParentId == null ? null : categories.FirstOrDefault(p => p.Id == c.ParentId)?.Name
        }).ToList();

        return View("Category/Index", categoryVMs);
    }

    public IActionResult CreateParentCategory()
    {
        return View("Category/CreateParent");
    }

    public IActionResult CreateSubCategory()
    {
        var parentCategories = _db.Categories
                                  .Where(c => c.ParentId == null)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id,
                                      Text = c.Name
                                  }).ToList();

        var vm = new CategoryVM
        {
            ParentCategoryOptions = parentCategories
        };

        return View("Category/CreateSub", vm);
    }

    public IActionResult EditParentCategory(string id)
    {
        var category = _db.Categories.Find(id);
        if (category == null) return NotFound();

        return View("Category/EditParent", category);
    }

    public IActionResult EditSubCategory(string id)
    {
        // 1. 找到当前正在编辑的子分类
        var category = _db.Categories.Find(id);
        if (category == null) return NotFound();

        // 2. 加载所有主分类作为下拉列表选项（即 ParentId == null 的）
        var parentCategories = _db.Categories
                                  .Where(c => c.ParentId == null)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id,
                                      Text = c.Name
                                  }).ToList();

        // 3. 构建视图模型并设置默认值
        var vm = new CategoryVM
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId, // ✅ 默认选中的值
            ParentCategoryOptions = parentCategories
        };

        // 4. 渲染视图
        return View("Category/EditSub", vm);
    }

    public IActionResult DeleteCategory(string id)
    {
        var category = _db.Categories.Find(id);
        if (category == null) return NotFound();
        return View("Category/Delete", category);
    }

    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateCategory(CategoryVM vm)
    {
        // Model验证
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Id = GenerateCategoryId(),
                Name = vm.Name,
                ParentId = vm.ParentId // 如果是主分类则ParentId为null
            };

            _db.Categories.Add(category);
            _db.SaveChanges();
        }
        else
        {
            DebugModelStateErrors();
        }

        return RedirectToAction("Categories");
    }

    [HttpPost]
    public IActionResult EditParentCategory(Category category)
    {
        if (ModelState.IsValid)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            return RedirectToAction("Categories");
        }
        return View("Category/EditParent");
    }

    [HttpPost]
    public IActionResult EditSubCategory(CategoryVM vm)
    {
        if (vm.Id == null)
        {
            ModelState.AddModelError("Id", "分类 ID 不能为空");
            return View("Category/EditSub", vm);
        }
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Id = vm.Id,
                Name = vm.Name,
                ParentId = vm.ParentId // 如果是主分类则ParentId为null
            };

            _db.Categories.Update(category);
            _db.SaveChanges();
            return RedirectToAction("Categories");
        }
        return View("Category/EditSub", vm);
    }

    [HttpPost, ActionName("DeleteCategory")]
    public IActionResult DeleteCategoryConfirmed(string id)
    {
        var category = _db.Categories.Find(id);
        if (category != null)
        {
            _db.Categories.Remove(category);
            _db.SaveChanges();
        }
        return RedirectToAction("Categories");
    }

    #endregion

    #region Functions
    private string GenerateCategoryId()
    {
        // 查询当前已有的最大编号
        var lastCategory = _db.Categories
            .Where(c => c.Id.StartsWith("C"))
            .OrderByDescending(c => c.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastCategory != null)
        {
            string lastNumberStr = lastCategory.Id.Substring(3);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"C{nextNumber.ToString("D3")}";  // 例如 JOB001、JOB002
    }

    public IActionResult CheckCategoryId(string id)
    {
        bool exists = _db.Categories.Any(c => c.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }

    #endregion
    #endregion

    // Institution =============================================================================================================== Institution
    #region Institution

    #region GET
    public IActionResult Institutions()
    {
        var institutions = _db.Institutions.ToList();
        return View("Institution/Index", institutions);
    }

    public IActionResult CreateInstitution()
    {
        return View("Institution/Create");
    }

    public IActionResult EditInstitution(string id)
    {
        var institution = _db.Institutions.Find(id);
        if (institution == null) return NotFound();
        return View("Institution/Edit", institution);
    }

    public IActionResult DeleteInstitution(string id)
    {
        var institution = _db.Institutions.Find(id);
        if (institution == null) return NotFound();
        return View("Institution/Delete", institution);
    }
    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateInstitution(InstitutionVM vm)
    {
        if (ModelState.IsValid)
        {
            var institution = new Institution
            {
                Id = GenerateInstitutionId(),
                Name = vm.Name
            };
            _db.Institutions.Add(institution);
            _db.SaveChanges();
            return RedirectToAction("Institutions");
        }
        return View("Institution/Create", vm);
    }

    [HttpPost]
    public IActionResult EditInstitution(Institution institution)
    {
        if (ModelState.IsValid)
        {
            _db.Institutions.Update(institution);
            _db.SaveChanges();
            return RedirectToAction("Institutions");
        }
        return View("Institution/Edit", institution);
    }

    [HttpPost, ActionName("DeleteInstitution")]
    public IActionResult DeleteInstitutionConfirmed(string id)
    {
        var institution = _db.Institutions.Find(id);
        if (institution != null)
        {
            _db.Institutions.Remove(institution);
            _db.SaveChanges();
        }
        return RedirectToAction("Institutions");
    }
    #endregion

    #region Functions
    private string GenerateInstitutionId()
    {
        // 查询当前已有的最大编号
        var lastInstitution = _db.Institutions
            .Where(i => i.Id.StartsWith("I"))
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastInstitution != null)
        {
            string lastNumberStr = lastInstitution.Id.Substring(3);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"I{nextNumber.ToString("D3")}";  // 例如 JOB001、JOB002
    }

    public IActionResult CheckInstitutionId(string id)
    {
        bool exists = _db.Institutions.Any(i => i.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }
    #endregion
    #endregion

    // Qualification =============================================================================================================== Qualification
    #region Qualification

    #region GET
    public IActionResult Qualifications()
    {
        var qualifications = _db.Qualifications.ToList();
        return View("Qualification/Index", qualifications);
    }

    public IActionResult CreateQualification()
    {
        return View("Qualification/Create");
    }

    public IActionResult EditQualification(string id)
    {
        var qualification = _db.Qualifications.Find(id);
        if (qualification == null) return NotFound();
        return View("Qualification/Edit", qualification);
    }

    public IActionResult DeleteQualification(string id)
    {
        var qualification = _db.Qualifications.Find(id);
        if (qualification == null) return NotFound();
        return View("Qualification/Delete", qualification);
    }
    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateQualification(QualificationVM vm)
    {
        if (ModelState.IsValid)
        {
            var qualification = new Qualification
            {
                Id = GenerateQualificationId(),
                Name = vm.Name
            };
            _db.Qualifications.Add(qualification);
            _db.SaveChanges();
            return RedirectToAction("Qualifications");
        }
        return View("Qualification/Create", vm);
    }

    [HttpPost]
    public IActionResult EditQualification(Qualification qualification)
    {
        if (ModelState.IsValid)
        {
            _db.Qualifications.Update(qualification);
            _db.SaveChanges();
            return RedirectToAction("Qualifications");
        }
        return View("Qualification/Edit", qualification);
    }

    [HttpPost, ActionName("DeleteQualification")]
    public IActionResult DeleteQualificationConfirmed(string id)
    {
        var qualification = _db.Qualifications.Find(id);
        if (qualification != null)
        {
            _db.Qualifications.Remove(qualification);
            _db.SaveChanges();
        }
        return RedirectToAction("Qualifications");
    }
    #endregion

    #region Functions
    private string GenerateQualificationId()
    {
        var lastQualification = _db.Qualifications
            .Where(q => q.Id.StartsWith("Q"))
            .OrderByDescending(q => q.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastQualification != null)
        {
            string lastNumberStr = lastQualification.Id.Substring(1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"Q{nextNumber.ToString("D3")}";
    }

    public IActionResult CheckQualificationId(string id)
    {
        bool exists = _db.Qualifications.Any(q => q.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }
    #endregion
    #endregion

    // Education =============================================================================================================== Education
    #region Education

    #region GET
    public IActionResult Educations()
    {
        var educations = _db.Educations
            .Include(e => e.User) // 通过导航属性加载用户
            .ToList();
        return View("Education/Index", educations);
    }

    public IActionResult CreateEducation()
    {
        var vm = new EducationVM
        {
            UserOptions = _db.Users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.Name
            }).ToList(),

            QualificationOptions = _db.Qualifications.Select(q => new SelectListItem
            {
                Value = q.Name,
                Text = q.Name
            }).ToList(),

            InstitutionOptions = _db.Institutions.Select(i => new SelectListItem
            {
                Value = i.Name,
                Text = i.Name
            }).ToList()
        };

        return View("Education/Create", vm);
    }


    public IActionResult EditEducation(string id)
    {
        var education = _db.Educations
                    .Include(e => e.User)
                    .FirstOrDefault(e => e.Id == id);
        if (education == null) return NotFound();

        var vm = new EducationVM
        {
            // 用于提交更新
            Id = education.Id,
            UserId = education.UserId,

            // 用于显示编辑时默认值
            UserName = education.User.Name,
            Qualification = education.Qualification,
            Institution = education.Institution,

            // 下拉列表选项
            QualificationOptions = _db.Qualifications.Select(q => new SelectListItem
            {
                Value = q.Name,
                Text = q.Name
            }).ToList(),

            InstitutionOptions = _db.Institutions.Select(i => new SelectListItem
            {
                Value = i.Name,
                Text = i.Name
            }).ToList()
        };

        return View("Education/Edit", vm);
    }

    public IActionResult DeleteEducation(string id)
    {
        var education = _db.Educations.Find(id);
        if (education == null) return NotFound();
        return View("Education/Delete", education);
    }
    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateEducation(EducationVM vm)
    {
        if (ModelState.IsValid)
        {
            var education = new Education
            {
                Id = GenerateEducationId(),
                UserId = vm.UserId,
                Qualification = vm.Qualification,
                Institution = vm.Institution
            };
            _db.Educations.Add(education);
            _db.SaveChanges();
            return RedirectToAction("Educations");
        }
        return View("Education/Create", vm);
    }

    [HttpPost]
    public IActionResult EditEducation(EducationVM vm)
    {
        if (vm.Id == null)
        {
            ModelState.AddModelError("Id", "ID 不能为空");
            return View("Education/Edit", vm);
        }
        if (ModelState.IsValid)
        {
            var education = new Education
            {
                Id = vm.Id,
                UserId = vm.UserId,
                Qualification = vm.Qualification,
                Institution = vm.Institution
            };
            _db.Educations.Update(education);
            _db.SaveChanges();
            return RedirectToAction("Educations");
        }

        DebugModelStateErrors();
        return View("Education/Edit", vm);
    }

    [HttpPost, ActionName("DeleteEducation")]
    public IActionResult DeleteEducationConfirmed(string id)
    {
        var education = _db.Educations.Find(id);
        if (education != null)
        {
            _db.Educations.Remove(education);
            _db.SaveChanges();
        }
        return RedirectToAction("Educations");
    }
    #endregion

    #region Functions
    private string GenerateEducationId()
    {
        var lastEducation = _db.Educations
            .Where(e => e.Id.StartsWith("E"))
            .OrderByDescending(e => e.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastEducation != null)
        {
            string lastNumberStr = lastEducation.Id.Substring(1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"E{nextNumber.ToString("D3")}";
    }

    public IActionResult CheckEducationId(string id)
    {
        bool exists = _db.Educations.Any(e => e.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }
    #endregion

    #endregion

    // JobExperience ===================================================================================================== JobExperience
    #region JobExperience
    #region GET
    public IActionResult JobExperiences()
    {
        var experiences = _db.JobExperiences.Include(j => j.User).ToList();
        return View("JobExperience/Index", experiences);
    }

    public IActionResult CreateJobExperience()
    {
        var now = DateTime.Now;

        var vm = new JobExperienceVM
        {
            UserOptions = _db.Users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.Name
            }).ToList(),

            // ✅ 下拉列表选项
            YearOptions = GenerateYearOptions(),

            MonthOptions = GenerateMonthOptions(),

            StartYear = now.Year,
            StartMonth = now.Month,
            EndYear = now.Year,
            EndMonth = now.Month,
        };

        return View("JobExperience/Create", vm);
    }

    public IActionResult EditJobExperience(string id)
    {
        var experience = _db.JobExperiences
            .Include(j => j.User) // 通过导航属性加载用户
            .FirstOrDefault(j => j.Id == id);
        if (experience == null) return NotFound();

        var now = DateTime.Now;

        var vm = new JobExperienceVM
        {
            // ✅ 下拉列表选项
            YearOptions = GenerateYearOptions(),
            MonthOptions = GenerateMonthOptions(),

            // 用于提交更新
            Id = experience.Id,
            UserId = experience.UserId,

            JobTitle = experience.JobTitle,
            CompanyName = experience.CompanyName,
            StartYear = experience.StartYear,
            StartMonth = experience.StartMonth,
            EndYear = experience.EndYear,
            EndMonth = experience.EndMonth,
            StillInRole = experience.StillInRole,
            UserName = experience.User.Name, // 显示用户名
        };

        return View("JobExperience/Edit", vm);
    }

    public IActionResult DeleteJobExperience(string id)
    {
        var experience = _db.JobExperiences.Find(id);
        if (experience == null) return NotFound();
        return View("JobExperience/Delete", experience);
    }
    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateJobExperience(JobExperienceVM vm)
    {
        if (ModelState.IsValid)
        {
            var je = new JobExperience
            {
                Id = GenerateJobExperienceId(),
                UserId = vm.UserId,
                JobTitle = vm.JobTitle,
                CompanyName = vm.CompanyName,
                StartYear = vm.StartYear,
                StartMonth = vm.StartMonth,
                EndYear = vm.EndYear,
                EndMonth = vm.EndMonth,
                StillInRole = vm.StillInRole,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.JobExperiences.Add(je);
            _db.SaveChanges();

            return RedirectToAction(actionName: "JobExperiences");
        }

        DebugModelStateErrors();
        return View("JobExperience/Create", vm);
    }

    [HttpPost]
    public IActionResult EditJobExperience(JobExperienceVM vm)
    {
        if (ModelState.IsValid)
        {
            var jobExp = _db.JobExperiences.Find(vm.Id);
            if (jobExp == null) return NotFound();

            jobExp.UserId = vm.UserId;
            jobExp.JobTitle = vm.JobTitle;
            jobExp.CompanyName = vm.CompanyName;
            jobExp.StartYear = vm.StartYear;
            jobExp.StartMonth = vm.StartMonth;
            jobExp.EndYear = vm.EndYear;
            jobExp.EndMonth = vm.EndMonth;
            jobExp.StillInRole = vm.StillInRole;
            jobExp.UpdatedAt = DateTime.UtcNow;

            _db.JobExperiences.Update(jobExp);
            _db.SaveChanges();

            return RedirectToAction("JobExperiences");
        }

        DebugModelStateErrors();
        return View("JobExperience/Edit", vm);
    }

    [HttpPost, ActionName("DeleteJobExperience")]
    public IActionResult DeleteJobExperienceConfirmed(string id)
    {
        var jobExp = _db.JobExperiences.Find(id);
        if (jobExp != null)
        {
            _db.JobExperiences.Remove(jobExp);
            _db.SaveChanges();
        }

        return RedirectToAction("JobExperiences");
    }
    #endregion

    #region Functions
    private string GenerateJobExperienceId()
    {
        var last = _db.JobExperiences
            .Where(j => j.Id.StartsWith("JE"))
            .OrderByDescending(j => j.Id)
            .FirstOrDefault();

        int next = 1;
        if (last != null)
        {
            string numberStr = last.Id.Substring(2);
            if (int.TryParse(numberStr, out int lastNumber))
            {
                next = lastNumber + 1;
            }
        }

        return $"JE{next.ToString("D3")}";
    }

    public IActionResult CheckJobExperienceId(string id)
    {
        bool exists = _db.JobExperiences.Any(j => j.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }
    #endregion
    #endregion






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

    // 生成年份和月份的下拉列表选项
    private List<SelectListItem> GenerateYearOptions()
    {
        var currentYear = DateTime.Now.Year;
        return Enumerable.Range(currentYear - 50, 50) // 50年前到今年
            .Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() })
            .Reverse() // 让最新年份排最前
            .ToList();
    }

    private List<SelectListItem> GenerateMonthOptions()
    {
        return Enumerable.Range(1, 12)
            .Select(m => new SelectListItem { Text = m.ToString("D2"), Value = m.ToString() })
            .ToList();
    }
}

public class TestDashboardViewModel
{
    public int UserCount { get; set; }
    public int JobCount { get; set; }
    public int ApplicationCount { get; set; }
    public int CategoryCount { get; set; }
    public int QualificationCount { get; set; }
    public int InstitutionCount { get; set; }
    public int NotificationCount { get; set; }
    public int EducationCount { get; set; }
    public int JobExperienceCount { get; set; }

}