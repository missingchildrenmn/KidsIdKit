
using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Core.Data;

public class SocialMediaAccount
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Platform")]
    public string? Platform { get; set; }

    [Required]
    [Display(Name = "User Name")]
    public string? UserName { get; set; }

    [Required]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}
