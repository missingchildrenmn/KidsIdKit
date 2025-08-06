using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Data;

public class ChildDetails 
{
    [Required]
    [Display(Name="Given name")]

    public string? GivenName { get; set; }
    [Display(Name = "Nickname")]

    public string? NickName { get; set; }

    [Display(Name = "Additional name")]
    public string? AdditionalName { get; set; }

    public string Names 
    { 
        get
        {
            var names = new List<string?> { NickName, AdditionalName };
            return string.Join(", ", names.Where(n => !string.IsNullOrWhiteSpace(n)));
        }
    }

    [Required]
    [Display(Name = "Family name")]
    public string? FamilyName { get; set; }

    public DateTime Birthday { get; set; } = DateTime.Today;
    public int Age {  get => DateTime.Today.Year - Birthday.Year; }

    [Display(Name = "Phone number")]

    public string? PhoneNumber { get; set; }

    public Photo Photo { get; set; } = new();
}
