using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Shared.Data;

public class DistinguishingFeature
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Photo")]
    public Photo Photo { get; set; } = new();
}
