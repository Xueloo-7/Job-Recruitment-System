namespace Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class LoginVM
{
    [StringLength(maximumLength: 100), EmailAddress]
    public string Email { get; set; }

    [StringLength(100, MinimumLength =5), DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
