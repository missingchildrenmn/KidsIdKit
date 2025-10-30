using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Shared.Data
{
    public class CareProvider : Person
    {
        [Display(Name = "Clinic Name")]
        public string? ClinicName { get; set; }

        [Display(Name = "Role Description")]
        public string? CareRoleDescription { get; set; }
    }
}
