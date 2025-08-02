using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using System.Diagnostics;

public class UserController : Controller
{
    private readonly DB _context;

    public UserController(DB context)
    {
        _context = context;
    }
    //要改的
    private string GetLoggedInUserId()
    {
        // 假设你有登录系统
        // 这里直接示例一个 userId
        return "1";
    }

    private User GetUserProfileData()
    {
        var userId = GetLoggedInUserId();
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }


    // 显示编辑页面
    public IActionResult Edit()
    {
        var user = GetUserProfileData();
        if (user == null)
        {
            ViewBag.Message = "Please fill in your info.";
            return View(new User());
        }

        return View(user);
    }

    // 保存资料（处理表单提交）
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User model)
    {
        var userId = GetLoggedInUserId();
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (existingUser != null)
        {
            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.Location = model.Location;
            existingUser.PhoneNumber = model.PhoneNumber;

            _context.Users.Update(existingUser);
        }
        else
        {
            Debug.WriteLine("sss");
            model.Id = userId;
            _context.Users.Add(model);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Profile");
    }
}