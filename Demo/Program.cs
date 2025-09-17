global using Demo;
global using Demo.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");
builder.Services.AddScoped<Helper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "DefaultCookie";
    options.DefaultSignInScheme = "DefaultCookie";
    options.DefaultChallengeScheme = "DefaultCookie";
})
.AddCookie("DefaultCookie", options => { 
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddCookie("AdminCookie", options => { 
    options.LoginPath = "/Admin/Login";
});

builder.Services.AddHttpContextAccessor();


// 加入 Session 服务
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session有效期
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 使用 Session
app.UseSession(); 
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.Run();