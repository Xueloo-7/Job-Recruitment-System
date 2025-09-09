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
.AddCookie("DefaultCookie", options => { options.LoginPath = "/Account/Login"; })
.AddCookie("AdminCookie", options => { options.LoginPath = "/Admin/Login"; });

builder.Services.AddHttpContextAccessor();


var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.Run();

// test