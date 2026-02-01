using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Domain.Entities;

public class User
{
    [Key]
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Username { get; set; } = default!;
    
    [Required]
    public string Password { get; set; } = default!; // Note: In a real app, store a Hash, not plain text!
    
    public string[] Roles { get; set; } = Array.Empty<string>();
}