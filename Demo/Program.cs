global using Demo;
global using Demo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews();

services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");
services.AddScoped<Helper>();

services.AddAuthentication(options =>
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
services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); // Google OAuth 2.0 用

// 加入 Session 服务
services.AddDistributedMemoryCache();
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session有效期
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 在这里注册你需要的服务
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<IEventDispatcher, EventDispatcher>();

// 注册事件处理器
services.AddScoped<IEventHandler<JobAppliedEvent>, JobAppliedEventHandler>(); // 
services.AddScoped<IEventHandler<InterviewScheduledEvent>, InterviewScheduledEventHandler>();
services.AddScoped<IEventHandler<JobExpiredEvent>, JobExpiredEventHandler>();
services.AddScoped<IEventHandler<ApplicationStatusChangedEvent>, ApplicationStatusChangedEventHandler>();
services.AddScoped<IEventHandler<JobReviewedEvent>, JobReviewedEventHandler>();
services.AddScoped<IEventHandler<AccountChangedEvent>, AccountChangedEventHandler>();


var app = builder.Build();

// 中间件顺序很重要！
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 必须在 UseAuthorization 之前
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();