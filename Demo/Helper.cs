using Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;



namespace Demo;

public class Helper
{
    private readonly IWebHostEnvironment en;
    private readonly IHttpContextAccessor ct;
    private readonly DB _db;

    // TODO
    public Helper(IWebHostEnvironment en, IHttpContextAccessor ct, DB _content)
    {
        this.en = en;
        this.ct = ct;
        _db = _content;
    }


    // ------------------------------------------------------------------------
    // Photo Upload
    // ------------------------------------------------------------------------

    public string ValidatePhoto(IFormFile f)
    {
        var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
        var reName = new Regex(@"^.+\.(jpeg|jpg|png)$", RegexOptions.IgnoreCase);

        if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
        {
            return "Only JPG and PNG photo is allowed.";
        }
        else if (f.Length > 1 * 1024 * 1024)
        {
            return "Photo size cannot more than 1MB.";
        }

        return "";
    }

    public string SavePhoto(IFormFile f, string folder)
    {
        // TODO
        var file = Guid.NewGuid().ToString("n") + ".jpg";
        var path = Path.Combine(en.WebRootPath, folder, file);

        var options = new ResizeOptions
        {
            Size = new(200, 200),
            Mode = ResizeMode.Crop,
        };

        using var stream = f.OpenReadStream();
        using var img = Image.Load(stream);
        img.Mutate(x => x.Resize(options));
        img.Save(path);

        return file;
    }

    public void DeletePhoto(string file, string folder)
    {
        file = Path.GetFileName(file);
        var path = Path.Combine(en.WebRootPath, folder, file);
        File.Delete(path);
    }



    // ------------------------------------------------------------------------
    // Security Helper Functions
    // ------------------------------------------------------------------------

    // TODO
    private readonly PasswordHasher<object> ph = new();

    public string HashPassword(string password)
    {
        // TODO
        return ph.HashPassword(0, password);
    }

    public bool VerifyPassword(string hash, string password)
    {
        // TODO
        return ph.VerifyHashedPassword(0, hash, password)
            == PasswordVerificationResult.Success;
    }

    public void SignIn(User user, bool rememberMe)
    {
        var scheme = user.Role == Role.Admin ? "AdminCookie" : "DefaultCookie";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        ct.HttpContext!.SignInAsync(scheme, principal, properties);
    }



    public void SignOut()
    {
        ct.HttpContext!.SignOutAsync("DefaultCookie");
    }

    public void AdminSignOut()
    {
        ct.HttpContext!.SignOutAsync("AdminCookie");
    }

    public string RandomPassword()
    {
        string s = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string password = "";

        // TODO
        Random r = new();

        for (int i = 1; i <= 10; i++)
        {
            password += s[r.Next(s.Length)];
        }

        return password;
    }


    // ------------------------------------------------------------------------
    // DB Functions
    // ------------------------------------------------------------------------

    /// <summary>
    /// 生成通用的流水号 ID
    /// </summary>
    /// <typeparam name="TEntity">实体类型，必须有 Id 属性</typeparam>
    /// <param name="dbSet">EF 的 DbSet</param>
    /// <param name="prefix">前缀，比如 "A" / "N"</param>
    /// <param name="length">数字部分长度，默认 3 => A001</param>
    public static string GenerateId<TEntity>(DbSet<TEntity> dbSet, string prefix, int length = 3)
        where TEntity : class, IHasId
    {
        var last = dbSet
            .Where(t => t.Id.StartsWith(prefix))
            .OrderByDescending(u => u.Id)
            .FirstOrDefault();

        int next = 1;
        if (last != null)
        {
            string numberStr = last.Id.Substring(prefix.Length);
            if (int.TryParse(numberStr, out int lastNumber))
            {
                next = lastNumber + 1;
            }
        }

        return $"{prefix}{next.ToString($"D{length}")}";
    }


}

/// <summary>
/// 保证实体有 Id 属性
/// </summary>
public interface IHasId
{
    string Id { get; set; }
}

// ------------------------------------------------------------------------
// Debug Functions
// ------------------------------------------------------------------------
public static class ModelStateExtensions
{
    public static void DebugErrors(this ModelStateDictionary modelState)
    {
        foreach (var entry in modelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"❌ Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }

}

