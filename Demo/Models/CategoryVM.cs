using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models;
#nullable disable warnings

public class CategoryVM
{
    public string? Id { get; set; } // 不参与验证，由系统生成
    public string Name { get; set; }
    public string? ParentName { get; set; }
    public string Type => string.IsNullOrEmpty(ParentName) ? "主分类" : "次分类";
    public string? ParentId { get; set; } // 用于提交选中的父分类 ID
    public List<SelectListItem>? ParentCategoryOptions { get; set; } // 用于生成下拉框
}
