using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Demo.Models;

public class AccountController : Controller
{
    private readonly DB _context;

    public AccountController(DB context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, bool rememberMe)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email");
            return View("signIn"); // 重新显示 Sign In 页面
        }

        // 创建 claims 身份
        var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name ?? "Unknown"),
        new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

        return RedirectToAction("Index", "Home"); // 登录后跳转主页
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("SignIn");
    }
}
