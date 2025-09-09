namespace Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class SearchViewModel
{
    public string Placeholder { get; set; } = "请输入搜索内容";
    public string Action { get; set; } // 提交到的 Action
    public string Controller { get; set; } // 提交到的 Controller
    public string Query { get; set; } // 搜索关键字
    public string Method { get; set; } = "GET"; // GET 或 POST
}