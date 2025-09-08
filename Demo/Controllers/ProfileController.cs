using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Controllers
{
    [Authorize(AuthenticationSchemes = "DefaultCookie")]
    public class ProfileController : Controller
    {
        private readonly DB db;

        public ProfileController(DB context)
        {
            db = context;
        }

        // 测试数据
        private static User demoUser = new User
        {
            FirstName = "no data found",
            LastName = "",
            Email = "no data found",
            Location = "no data found",
            PhoneNumber = "no data found"
        };

        private User GetCurrentUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return demoUser;

            var user = db.Users.Find(userId);
            return user ?? demoUser;
        }

        // 首页
        public IActionResult Index(string? userId = "")
        {
            User user = GetCurrentUser(userId);
            return View(user);
        }

        // ------------------- Profile -------------------
        public IActionResult Profile(string? userId)
        {
            return PartialView("_Profile", GetCurrentUser(userId));
        }

        public IActionResult EditProfilePartial(string? userId)
        {
            return PartialView("_EditProfile", GetCurrentUser(userId));
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // 手动更新允许修改的字段
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.Location = updatedUser.Location;

            db.SaveChanges();
            return PartialView("_Profile", existingUser);
        }


        // ------------------- Summary -------------------
        public IActionResult Summary(string? userId)
        {
            return PartialView("_Summary", GetCurrentUser(userId));
        }

        [HttpGet]
        public IActionResult EditSummaryPartial(string? userId)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_EditSummary", user);
        }

        [HttpPost]
        public IActionResult EditSummary(User updatedUser)
        {
            db.Users.Update(updatedUser);
            db.SaveChanges();
            return PartialView("_Summary", updatedUser);
        }

        // ------------------- Career History -------------------
        public IActionResult CareerHistory(string? userId)
        {
            var jobs = db.JobExperiences.Where(j => j.UserId == userId).ToList();

            return PartialView("_CareerHistory", jobs);
        }

        [HttpGet]
        public IActionResult EditCareerHistory(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var job = db.JobExperiences.FirstOrDefault(j => j.UserId == userId);
            if (job == null)
            {
                return NotFound();
            }

            return PartialView("_EditCareerHistory", job);
        }

        [HttpPost]
        public IActionResult EditCareerHistory(JobExperience jobExperience)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditCareerHistory", jobExperience);
            }

            db.JobExperiences.Update(jobExperience);
            db.SaveChanges();

            // 编辑完成后返回 CareerHistory
            var jobs = db.JobExperiences
                         .Where(j => j.UserId == jobExperience.UserId)
                         .ToList();

            return PartialView("_CareerHistory", jobs);
        }

        // ------------------- Education -------------------

        // 显示用户的所有 Education 记录
        public IActionResult Education(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return PartialView("_Education", Enumerable.Empty<Education>());

            var educations = db.Educations.Where(e => e.UserId == userId).ToList();

            return PartialView("_Education", educations);
        }

        // 显示编辑某个 Education 的表单
        public IActionResult EditEducationPartial(string? id)
        {
            var education = db.Educations.FirstOrDefault(e => e.Id == id);
            if (education == null)
            {
                return NotFound();
            }
                

            return PartialView("_EditEducation", education);
        }

        // 保存 Education 编辑后的数据
        [HttpPost]
        public IActionResult EditEducation(Education updatedEducation)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditEducation", updatedEducation);

            db.Educations.Update(updatedEducation);
            db.SaveChanges();

            var educations = db.Educations.Where(e => e.UserId == updatedEducation.UserId).ToList();

            return PartialView("_Education", educations);
        }
    }
}
