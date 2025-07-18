using Demo.Models;
using Microsoft.AspNetCore.Mvc;

//---Put in first because no database in Models---


//public IActionResult SearchAjax(string? keyword, string? location)
//{
//    keyword = keyword?.Trim().ToLower() ?? "";
//    location = location?.Trim().ToLower() ?? "";

//    var result = db.Jobs
//        .Where(j =>
//            (string.IsNullOrEmpty(keyword) || j.Title.ToLower().Contains(keyword)) &&
//            (string.IsNullOrEmpty(location) || j.Location.ToLower().Contains(location)))
//        .ToList();

//    return PartialView("_JobListPartial", result);
//}