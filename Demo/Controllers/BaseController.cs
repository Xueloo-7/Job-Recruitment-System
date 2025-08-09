using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

public class BaseController : Controller
{ 
    protected string GetCurrentUserId()
    {
        // 从当前用户的 Claims 中获取 UserId
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (string.IsNullOrEmpty(userId))
        {
            // 如果没有找到 UserId，可能是未登录或其他问题
            Debug.WriteLine("UserId not found in claims.");
            return string.Empty; // 或者抛出异常，视具体需求而定
        }
        return userId;
    }
}