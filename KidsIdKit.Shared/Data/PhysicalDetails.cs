using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Shared.Data
{
    public class PhysicalDetails
    {
        [Display(Name = "Height")]
        public string? Height { get; set; }

        [Display(Name = "Weight")]
        public string? Weight { get; set; }

        [Display(Name = "Measurement date")]
        public DateTime MeasurementDate { get; set; } = DateTime.Today;

        [Display(Name = "Hair color")]
        public string? HairColor { get; set; }

        [Display(Name = "Hair style")]
        public string? HairStyle { get; set; }

        [Display(Name = "Eye color")]
        public string? EyeColor { get; set; }

        [Display(Name = "Eye glasses")]
        public bool EyeGlasses { get; set; }

        [Display(Name = "Wears contacts")]
        public bool EyeContacts { get; set; }

        [Display(Name = "Skin tone")]
        public string? SkinTone { get; set; }

        [Display(Name = "Racial/ethnic identity")]
        public string? RacialEthnicIdentity { get; set; }

        [Display(Name = "Sex")]
        public string? Sex { get; set; }

        [Display(Name = "Gender identity")]
        public string? GenderIdentity { get; set; }
    }
}
