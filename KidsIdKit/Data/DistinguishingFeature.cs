using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Data
{
    public class DistinguishingFeature
    {
        public int Id { get; set; }
        [Required]
        public string? Description { get; set; }
        public FileReference? FileReference { get; set; }
    }
}
