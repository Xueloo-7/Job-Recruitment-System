namespace Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

public class UserVM
{
    [Required, MaxLength(100)]
    public string PasswordHash { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    public Role Role { get; set; }
}