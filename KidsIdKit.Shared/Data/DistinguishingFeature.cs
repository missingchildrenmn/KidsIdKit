using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Data;

public class DistinguishingFeature
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Description")]     // 8-16-2025 - noticed that it was missing
    public string? Description { get; set; }

    [Display(Name = "Photo")]     // 8-16-2025 - noticed that it was missing
    public Photo Photo { get; set; } = new();
}
