using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RegisterRequest
{
    [Required] 
    public required string Email { get; set; }

    [Required]
    [MinLength(6)] 
    public required string Password { get; set; }

    [Required] 
    public required string FirstName { get; set; }

    [Required] 
    public required string LastName { get; set; }

    [Required] 
    public required string Role { get; set; }
}