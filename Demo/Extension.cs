using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace Demo;

public static class Extension
{
    public static bool isAjax(this HttpRequest request)
    {
        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }

    // Returns "active" if the current route matches the specified controller
    public static string IsActive(this IHtmlHelper html, string controller, string action = null)
    {
        var routeData = html.ViewContext.RouteData;
        var currentController = routeData.Values["controller"]?.ToString();
        var currentAction = routeData.Values["action"]?.ToString();

        if (!string.Equals(controller, currentController, System.StringComparison.OrdinalIgnoreCase))
            return "";

        if (string.IsNullOrEmpty(action) || string.Equals(action, currentAction, System.StringComparison.OrdinalIgnoreCase))
            return "active";

        return "";
    }


    public static bool IsValid(this ModelStateDictionary ms, string key)
    {
        return ms.GetFieldValidationState(key) == ModelValidationState.Valid;
    }


    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) // for keeping data between requests
    {
        tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T Get<T>(this ITempDataDictionary tempData, string key)
    {
        tempData.TryGetValue(key, out var o);
        return o == null ? default : JsonSerializer.Deserialize<T>((string)o);
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unkown User";
    }


    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString());
        var attribute = member[0].GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }
}
