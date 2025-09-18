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
            // 记录变更内容
            var changes = new List<string>();
            if (existingUser.FirstName != model.FirstName)
                changes.Add($"FirstName: {existingUser.FirstName} -> {model.FirstName}");
            if (existingUser.LastName != model.LastName)
                changes.Add($"LastName: {existingUser.LastName} -> {model.LastName}");
            if (existingUser.Location != model.Location)
                changes.Add($"Location: {existingUser.Location} -> {model.Location}");
            if (existingUser.PhoneNumber != model.PhoneNumber)
                changes.Add($"PhoneNumber: {existingUser.PhoneNumber} -> {model.PhoneNumber}");

            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.Location = model.Location;
            existingUser.PhoneNumber = model.PhoneNumber;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            // audit log
            var log = new AuditLog
            {
                UserId = userId,
                TableName = "Users",
                ActionType = "Update",
                RecordId = existingUser.Id,
                Changes = changes.Count > 0 ? string.Join(", ", changes) : "No changes"
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        else
        {
            Debug.WriteLine("sss");
            model.Id = userId;
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            // audit log
            var log = new AuditLog
            {
                UserId = userId,
                TableName = "Users",
                ActionType = "Create",
                RecordId = model.Id,
                Changes = $"Created user: {model.Email}"
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Profile");
    }
}