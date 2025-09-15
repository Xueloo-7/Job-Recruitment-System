using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
namespace Demo.Controllers
{
    public class JobApplyController : Controller
    {
        private readonly DB db;
        private readonly IWebHostEnvironment en;
        private readonly Helper hp;


        public JobApplyController(DB db, IWebHostEnvironment en, Helper hp)
        {
            this.db = db;
            this.en = en;
            this.hp = hp;
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

        // 假设用户已经登录
        private User GetCurrentUser()
        {
            // 开发阶段临时使用
            return new User
            {
                Id = "U004",
                Name = "Sara Wong",
                Role = Role.JobSeeker
            };
        }

        private void LoadDropdowns(ApplyPageVM vm) // for reset top down menu 
        {

            vm.Application.Sources = Enum.GetValues(typeof(ApplicationSource))
                 .Cast<ApplicationSource>()
                  .Select(e => new SelectListItem
                  {
                      Value = e.ToString(),
                      Text = e.ToString()
                  })
                  .ToList();


            vm.Application.NoticeTimes = Enum.GetValues(typeof(NoticeTime))
                .Cast<NoticeTime>()
                 .Select(e => new SelectListItem
                 {
                     Value = e.ToString(),
                     Text = e.GetType() // 存储枚举值
                          .GetMember(e.ToString())
                          .First()
                          .GetCustomAttribute<DisplayAttribute>()?
                          .Name ?? e.ToString() // 用 Display 名称
                 })
                 .ToList();


            vm.Application.SalaryExpecteds = Enum.GetValues(typeof(SalaryExpected))
                .Cast<SalaryExpected>()
                 .Select(e => new SelectListItem
                 {
                     Value = e.ToString(), // 存储枚举值
                     Text = e.GetType()
                          .GetMember(e.ToString())
                          .First()
                          .GetCustomAttribute<DisplayAttribute>()?
                          .Name ?? e.ToString() // 用 Display 名称
                 })
                 .ToList();
        }


        public IActionResult Apply(string jobId)
        {
            var currentUser = GetCurrentUser(); // 模拟登录
            if (currentUser == null)
            {
                // 如果没有用户就重定向到登录页
                return RedirectToAction("Login", "Account");// 登录检查
            }

            var job = db.Jobs.Find(jobId);
            if (job == null)
                return NotFound();

            var vm = new ApplyPageVM
            {
                Application = new ApplicationVM
                {
                    JobId = job.Id,
                    UserId = currentUser.Id
                },
                Resume = new ResumeVM()
            };// create VM
            LoadDropdowns(vm); // load the list down menu 

            ViewBag.Job = job; // get the job data from job table 
            return View(vm);
        }


        [HttpPost]
        public IActionResult Apply(ApplyPageVM vm)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            // 手动补充 UserId
            vm.Application.UserId = currentUser.Id;
            vm.Resume.UserId = currentUser.Id;

            var job = db.Jobs.Find(vm.Application.JobId);
            if (job == null)
                return NotFound();

            // 表单验证
            if (!ModelState.IsValid)
            {
                DebugModelStateErrors();
                LoadDropdowns(vm);
                ViewBag.Job = job;               // ✅ 再次传给前端
                return View(vm);
            }


            bool Exist_Apply = db.Applications.Any(j => j.UserId == currentUser.Id && j.JobId == job.Id);
            if (Exist_Apply) // verify whether the job is exist for the specific account 
            {
                ModelState.AddModelError("", "You have already applied for this job.");
                ViewBag.Job = job;
                LoadDropdowns(vm);
                return View(vm);
            }

            // 按数字部分排序生成ID
            var maxId = db.Applications
                .Where(j => j.Id.StartsWith("A")) // 只取以 "J" 开头的岗位记录
                .Select(j => j.Id.Substring(1)) // 取 Id 的数字部分（去掉开头的 "J"）
                .AsEnumerable()                 // 把数据拉到内存中，EF Core 可以在内存中执行 int.Parse
                .Select(s => int.Parse(s))// 把字符串数字转换成整数
                .DefaultIfEmpty(0) // 如果数据库中没有记录，默认最大值为 0
                .Max(); // 取出最大值，用于生成下一个 ID

            int nextNumber = maxId + 1;
            vm.Application.Id = "A" + nextNumber.ToString("D3");


            // 正确访问 ResumeVM 里的 ImageFile
            if (vm.Resume == null || vm.Resume.ImageFile == null)
            {
                ModelState.AddModelError("Resume.ImageFile", "Please upload your resume file.");
                LoadDropdowns(vm);
                ViewBag.Job = job;
                return View(vm);
            }

            var error = hp.ValidatePhoto(vm.Resume.ImageFile);
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("Resume.ImageFile", error);
                LoadDropdowns(vm);
                ViewBag.Job = job;
                return View(vm);
            }

            string relativePath = null;

            try
            {
                // 保存文件到服务器，返回唯一文件名
                string ImageFileName = hp.SavePhoto(vm.Resume.ImageFile, "images/uploads/resume");

                // 拼接成相对路径保存到数据库
                relativePath = "images/uploads/resume/" + ImageFileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Resume.ImageFile", "Image save failed: " + ex.Message);
                LoadDropdowns(vm);
                ViewBag.Job = job;
                return View(vm);
            }

            // 保存 Resume（假设 Resume 表独立）
            var resume = new Resume
            {
                UserId = currentUser.Id,
                ImageUrl = relativePath

            };
            db.Resumes.Add(resume);
            db.SaveChanges(); // 保存后生成 resume.Id


            // 保存 Application
            var application = new Application
            {
                Id = vm.Application.Id,
                JobId = job.Id,
                UserId = currentUser.Id,
                Source = vm.Application.Source,
                NoticeTime = vm.Application.NoticeTime,
                SalaryExpected = vm.Application.SalaryExpected,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Applications.Add(application);
            db.SaveChanges();

            TempData["Info"] = "Application submitted successfully!";
            return RedirectToAction("ApplyList"); // 成功跳转页面
        }

        public IActionResult ApplyList()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var applications = db.Applications
                      .Include(a => a.Job)  // ✅ 必须 Include
                      .Where(a => a.UserId == currentUser.Id)
                      .OrderByDescending(a => a.CreatedAt)
                      .Select(a => new ApplicationListVM
                      {
                          ApplicationId = a.Id,
                          CreatedAt = a.CreatedAt,
                          Status = a.Status,
                          HiredDate = a.HiredDate,
                          JobTitle = a.Job.Title,
                          CompanyName = a.Job.CompanyName
                      })
                      .ToList();

            return View(applications);
        }


    }

}


