using System.ComponentModel.DataAnnotations;

namespace Nadin.WebAPI.Model;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}