using Demo;
using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
public class DemoJobController : Controller
{
    private readonly DB db;
    private readonly IWebHostEnvironment en;
    private readonly Helper hp;

    public DemoJobController(DB db, IWebHostEnvironment en, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    // Get: for listing job that u created
    public IActionResult Index()
    {
        var model = db.Jobs;
        return View(model);
    }

    public IActionResult Detail(string? id)
    {
        var model = db.Jobs.Find(id);

        if (model == null)
        {
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public bool CheckId(string id) // check the data redandency
    {

        return !db.Jobs.Any(j => j.Id == id);
    }

    // ============================================================ insert 1 ==============================
    // Get method used for jump to insert page 
    public IActionResult Insert1()
    {

        var vm = TempData.Get<JobVM1>("Insert1") ?? new JobVM1();
        // User 下拉列表
        vm.UserOptions = db.Users
            .Where(u => u.Role == Role.Employer)
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(), // string
                Text = u.Name
            })
            .ToList();

        // Category 下拉列表
        vm.CategoryOptions = db.Categories
            .Where(c => c.ParentId != null)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(), // string
                Text = c.Name
            })
            .ToList();

        // PayType & WorkType
        ViewBag.PayTypes = Enum.GetValues(typeof(PayType))// for listing payment method 
       .Cast<PayType>()
       .Select(e => new SelectListItem
       {
           Value = e.ToString(),
           Text = e.ToString()
       });

        ViewBag.WorkTypes = Enum.GetValues(typeof(WorkType))// for listing payment method 
      .Cast<WorkType>()
      .Select(e => new SelectListItem
      {
          Value = e.ToString(),
          Text = e.ToString()
      });

        LoadDropdowns(vm);
        return View(vm); //返回已经填充好的 vm
    }


    [HttpPost] // Post: for inserting new job
    public IActionResult Insert1(JobVM1 vm) //IFormFile? LogoFile)
    {
        bool TitleDuplicate = db.Jobs.Any(j => j.Title == vm.Title && j.CompanyName == vm.CompanyName);
        if (TitleDuplicate)
        {
            ModelState.AddModelError("Title", "A job with this title already exists in the same company.");
            return View(vm);
        }

        if (ModelState.IsValid)
        {
            // 按数字部分排序生成ID
            var maxId = db.Jobs
                .Where(j => j.Id.StartsWith("J")) // 只取以 "J" 开头的岗位记录
                .Select(j => j.Id.Substring(1)) // 取 Id 的数字部分（去掉开头的 "J"）
                .AsEnumerable()                 // 把数据拉到内存中，EF Core 可以在内存中执行 int.Parse
                .Select(s => int.Parse(s))// 把字符串数字转换成整数
                .DefaultIfEmpty(0) // 如果数据库中没有记录，默认最大值为 0
                .Max(); // 取出最大值，用于生成下一个 ID

            int nextNumber = maxId + 1;
            vm.Id = "J" + nextNumber.ToString("D3");

            TempData.Put("Insert1", vm);
            return RedirectToAction("Insert2"); // jumping 
        }

        LoadDropdowns(vm);
        return View(vm);  // return view with error msg

    }

    // ============================================================ insert 2 ==============================
    public IActionResult Insert2()
    {
        // Get method for second step
        var insert1 = TempData.Get<JobVM1>("Insert1"); // keep Insert1 data
        TempData.Keep("Insert1");

        var insert2 = TempData.Get<JobVM2>("Insert2") ?? new JobVM2()
        {
            Id = insert1.Id   // ✅ 带上 Id
        };

        // Promotion Options
        var promotions = db.Promotions.ToList();
        insert2.PromotionOptions = promotions.Select(p => new SelectListItem
        {
            Value = p.Id,
            Text = p.Name
        }).ToList();


        return View(insert2);
    }


    [HttpPost]
    public IActionResult Insert2(JobVM2 vm)
    {
        if (ModelState.IsValid)
        {
            TempData.Put("Insert2", vm);
            TempData.Keep("Insert1");
            return RedirectToAction("Insert3");
        }

        TempData.Keep("Insert1");
        vm.PromotionOptions = db.Promotions
         .Select(p => new SelectListItem
         {
             Value = p.Id,
             Text = p.Name
         })
         .ToList();
        return View(vm);
    }

    // ============================================================ insert 3 ==============================
    public IActionResult Insert3()
    {
        var insert1 = TempData.Get<JobVM1>("Insert1");
        var insert2 = TempData.Get<JobVM2>("Insert2");

        if (insert1 == null || insert2 == null)
            return RedirectToAction("Insert1");

        var insert3 = TempData.Get<JobVM3>("Insert3") ?? new JobVM3
        {
            Id = insert1.Id   // ✅ 带上 Id
        };

        TempData.Keep("Insert1");   // keep the data in TempData
        TempData.Keep("Insert2"); // keep the data in TempData
        return View(insert3);
    }


    [HttpPost]
    public IActionResult Insert3(JobVM3 vm)
    {
        // 图片验证
        if (vm.LogoFile == null)
        {
            ModelState.AddModelError("LogoFile", "Please upload an image.");
        }
        else
        {
            var e = hp.ValidatePhoto(vm.LogoFile);
            if (!string.IsNullOrEmpty(e))
                ModelState.AddModelError("LogoFile", e);
        }

        TempData.Keep("Insert1");
        TempData.Keep("Insert2");

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // 组合三步数据
        var Job = new CombineJobVM // combine data as one object
        {
            Insert1 = TempData.Get<JobVM1>("Insert1"),
            Insert2 = TempData.Get<JobVM2>("Insert2"),
            Insert3 = vm
        };

        string? relativePath = null;
        if (vm.LogoFile != null)
        {
            try
            {
                // 保存文件到服务器，返回唯一文件名
                string logoFileName = hp.SavePhoto(vm.LogoFile, "images/uploads/logo");

                // 拼接成相对路径保存到数据库
                relativePath = "images/uploads/logo/" + logoFileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("LogoFile", "Image save failed: " + ex.Message);
                return View(vm);
            }
        }

        var jobEntity = new Job
        {
            Id = Job.Insert1.Id,  // 使用第一步生成的ID
            Title = Job.Insert1.Title,
            CompanyName = Job.Insert1.CompanyName,
            Location = Job.Insert1.Location,
            UserId = Job.Insert1.UserId,
            CategoryId = Job.Insert1.CategoryId,
            PayType = Job.Insert1.PayType.Value,
            WorkType = Job.Insert1.WorkType.Value,
            SalaryMax = Job.Insert1.SalaryMax,
            SalaryMin = Job.Insert1.SalaryMin,
            Description = Job.Insert2.Description,
            Summary = Job.Insert2.Summary,
            PromotionId = Job.Insert2.PromotionId, // TODO: set PromotionId
            LogoImageUrl = relativePath,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        db.Jobs.Add(jobEntity); // 标记插入
        db.SaveChanges();


        TempData.Remove("Insert1"); //清理前两步缓存，避免占用内存。
        TempData.Remove("Insert2");
        TempData.Remove("Insert3"); // 一起清掉

        TempData["Info"] = "Job Inserted successfully.";
        return RedirectToAction("Index");
    }

    // ============================================================ update 1 ==============================
    public IActionResult Update1(string id = null)
    {
        // 优先从 TempData 拿
        var vm = TempData.Get<JobVM1>("Update1");

        if (vm == null)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var job = db.Jobs.FirstOrDefault(j => j.Id == id);
            if (job == null)
                return NotFound();

            vm = new JobVM1
            {
                Id = job.Id,
                Title = job.Title,
                CompanyName = job.CompanyName,
                Location = job.Location,
                UserId = job.UserId,
                CategoryId = job.CategoryId,
                PayType = job.PayType,
                WorkType = job.WorkType,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax
            };
        }

        LoadDropdowns(vm);
        TempData.Put("Update1", vm); // 更新 TempData
        TempData.Keep("Update2");
        TempData.Keep("Update3");

        return View(vm);
    }


    [HttpPost]
    public IActionResult Update1(JobVM1 vm)
    {
        if (ModelState.IsValid)
        {
            TempData.Put("Update1", vm);
            return RedirectToAction("Update2");
        }

        LoadDropdowns(vm);
        return View(vm);
    }

    // ============================================================ update 2 ==============================
    public IActionResult Update2()
    {
        var update1 = TempData.Get<JobVM1>("Update1");
        if (update1 == null)
            return RedirectToAction("Index");

        var vm = TempData.Get<JobVM2>("Update2") ?? new JobVM2
        {
            Id = update1.Id,
            Description = db.Jobs.Where(j => j.Id == update1.Id).Select(j => j.Description).FirstOrDefault(),
            Summary = db.Jobs.Where(j => j.Id == update1.Id).Select(j => j.Summary).FirstOrDefault(),
            PromotionId = db.Jobs.Where(j => j.Id == update1.Id).Select(j => j.PromotionId).FirstOrDefault()
        };

        vm.PromotionOptions = db.Promotions
            .Select(p => new SelectListItem { Value = p.Id, Text = p.Name })
            .ToList();

        TempData.Put("Update2", vm);
        TempData.Keep("Update1");
        TempData.Keep("Update3");

        return View(vm);
    }


    [HttpPost]
    public IActionResult Update2(JobVM2 vm)
    {
        if (ModelState.IsValid)
        {
            TempData.Put("Update2", vm);
            TempData.Keep("Update1");
            return RedirectToAction("Update3");
        }

        vm.PromotionOptions = db.Promotions
            .Select(p => new SelectListItem { Value = p.Id, Text = p.Name })
            .ToList();

        TempData.Keep("Update1");
        return View(vm);
    }

    // ============================================================ update 3 ==============================
    public IActionResult Update3()
    {
        var vm = TempData.Get<JobVM3>("Update3") ?? new JobVM3();

        TempData.Keep("Update1");
        TempData.Keep("Update2");

        var job1 = TempData.Get<JobVM1>("Update1");
        if (job1 != null)
        {
            var jobEntity = db.Jobs.FirstOrDefault(j => j.Id == job1.Id);
            if (jobEntity != null)
            {
                vm.ExistingLogoUrl = jobEntity.LogoImageUrl;
            }
        }

        TempData.Put("Update3", vm);
        return View(vm);
    }

    [HttpPost]
    public IActionResult Update3(JobVM3 vm)
    {
        TempData.Keep("Update1");
        TempData.Keep("Update2");

        // 验证 Logo 文件
        if (vm.LogoFile != null)
        {
            var e = hp.ValidatePhoto(vm.LogoFile);
            if (!string.IsNullOrEmpty(e))
                ModelState.AddModelError("LogoFile", e);
        }

        if (!ModelState.IsValid)
            return View(vm);

        // 合并三步数据
        var JobData = new CombineJobVM
        {
            Insert1 = TempData.Get<JobVM1>("Update1"),
            Insert2 = TempData.Get<JobVM2>("Update2"),
            Insert3 = vm
        };

        // 从数据库中加载现有实体
        var jobEntity = db.Jobs.FirstOrDefault(j => j.Id == JobData.Insert1.Id);
        if (jobEntity == null)
            return NotFound();

        // 更新字段
        jobEntity.Title = JobData.Insert1.Title;
        jobEntity.CompanyName = JobData.Insert1.CompanyName;
        jobEntity.Location = JobData.Insert1.Location;
        jobEntity.UserId = JobData.Insert1.UserId;
        jobEntity.CategoryId = JobData.Insert1.CategoryId;
        jobEntity.PayType = JobData.Insert1.PayType.Value;
        jobEntity.WorkType = JobData.Insert1.WorkType.Value;
        jobEntity.SalaryMax = JobData.Insert1.SalaryMax;
        jobEntity.SalaryMin = JobData.Insert1.SalaryMin;
        jobEntity.Description = JobData.Insert2.Description;
        jobEntity.Summary = JobData.Insert2.Summary;
        jobEntity.PromotionId = JobData.Insert2.PromotionId;

        // 如果上传了新 Logo，则更新图片
        if (vm.LogoFile != null)
        {
            string logoFileName = hp.SavePhoto(vm.LogoFile, "images/uploads/logo");
            jobEntity.LogoImageUrl = "images/uploads/logo/" + logoFileName;
        }

        jobEntity.UpdatedAt = DateTime.Now;

        db.SaveChanges();

        TempData.Remove("Update1");
        TempData.Remove("Update2");
        TempData["Info"] = "Job updated successfully.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(string? Id, string file)
    {
        var s = db.Jobs.Find(Id);
        if (s != null)
        {
            // TODO
            hp.DeletePhoto(file, "images/uploads/logo");
            db.Jobs.Remove(s);
            db.SaveChanges();
            TempData["Info"] = "Job ad delete successfully";
        }

        return Redirect(Request.Headers.Referer.ToString()); // TODO
    }
    //=======================================================================
    public IActionResult Subscription()
    {
        var promotions = db.Promotions.ToList();

        if (Request.isAjax())
        {
            return PartialView("_PromotionCard", promotions);
        }

        return View(promotions);
    }

    public IActionResult Details(string id)
    {
        var job = db.Jobs
            .Include(j => j.Category)
            .Include(j => j.User)
            .FirstOrDefault(j => j.Id == id);

        if (job == null) return NotFound();

        return PartialView("_JobDetails", job);
    }
    //===========================================================================

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

    private void LoadDropdowns(JobVM1 vm) // for reset top down menu 
    {

        vm.UserOptions = db.Users
       .Where(u => u.Role == Role.Employer)
       .Select(u => new SelectListItem
       {
           Value = u.Id.ToString(),
           Text = u.Name
       }).ToList();

        // Category 下拉列表
        vm.CategoryOptions = db.Categories
            .Where(c => c.ParentId != null)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

        ViewBag.PayTypes = Enum.GetValues(typeof(PayType))// for listing payment method 
       .Cast<PayType>()
       .Select(e => new SelectListItem
       {
           Value = e.ToString(),
           Text = e.ToString()
       });

        ViewBag.WorkTypes = Enum.GetValues(typeof(WorkType))// for listing payment method 
      .Cast<WorkType>()
      .Select(e => new SelectListItem
      {
          Value = e.ToString(),
          Text = e.ToString()
      });

    }

}

