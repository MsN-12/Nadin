using System.ComponentModel.DataAnnotations;

namespace Nadin.WebAPI.Model;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    [Required]
    [MinLength(6)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}