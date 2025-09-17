using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

public class BaseController : Controller
{
    protected void SetFlashMessage(FlashMessageType type, string message)
    {
        TempData["Flash.Type"] = type.ToString(); // Info / Success / Warning / Danger
        TempData["Flash.Message"] = message;
    }
}
public enum FlashMessageType
{
    Success,
    Info,
    Warning,
    Danger
}