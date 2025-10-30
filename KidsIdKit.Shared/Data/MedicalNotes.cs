using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Shared.Data
{
    public class MedicalNotes
    {
        [Display(Name = "MedicAlertInfo")]
        public string? MedicAlertInfo { get; set; }

        [Display(Name = "Allergies")]
        public string? Allergies { get; set; }

        [Display(Name = "Regular Medications")]
        public string? RegularMedications { get; set; }

        [Display(Name = "Psychiatric Medications")]
        public string? PsychMedications { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Inhaler")]
        public bool Inhaler { get; set; }

        [Display(Name = "Diabetic")]
        public bool Diabetic { get; set; }
    }
}
