using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Data
{
    public class MedicalNotes
    {
        [Display(Name = "MedicAlertInfo")]     // 8-16-2025 - noticed that it was missing
        public string? MedicAlertInfo { get; set; }

        [Display(Name = "Allergies")]     // 8-16-2025 - noticed that it was missing
        public string? Allergies { get; set; }

        [Display(Name = "Regular Medications")]     // 8-16-2025 - noticed that it was missing
        public string? RegularMedications { get; set; }

        [Display(Name = "Psychiatric Medications")]     // 8-16-2025 - noticed that it was missing
        public string? PsychMedications { get; set; }

        [Display(Name = "Notes")]     // 8-16-2025 - noticed that it was missing
        public string? Notes { get; set; }

        [Display(Name = "Inhaler")]     // 8-16-2025 - noticed that it was missing
        public bool Inhaler { get; set; }

        [Display(Name = "Diabetic")]     // 8-16-2025 - noticed that it was missing
        public bool Diabetic { get; set; }
    }
}
