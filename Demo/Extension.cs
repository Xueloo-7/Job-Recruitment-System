using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

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
}
