namespace Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


#nullable disable warnings

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

}
public class User
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Username { get; set; }
    [Required]
    [MaxLength(50)]
    public string PasswordHash { get; set; }
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}