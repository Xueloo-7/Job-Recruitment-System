global using Demo;
global using Demo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// 数据库
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");
builder.Services.AddScoped<Helper>();

// 身份认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "DefaultCookie";
    options.DefaultSignInScheme = "DefaultCookie";
    options.DefaultChallengeScheme = "DefaultCookie";
})
.AddCookie("DefaultCookie", options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddCookie("AdminCookie", options =>
{
    options.LoginPath = "/Admin/Login";
});

// 授权
builder.Services.AddAuthorization();

// HttpContext / HttpClient
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); // Google OAuth 2.0 用

var app = builder.Build();

// 中间件顺序很重要！
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 必须在 UseAuthorization 之前
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();