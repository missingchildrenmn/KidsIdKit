using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Core.Data
{
    public class Person 
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Given name")]
        public string? GivenName { get; set; }

        [Display(Name = "Nickname")]
        public string? NickName { get; set; }
       
        [Display(Name = "Family name")]
        public string? FamilyName { get; set; }
       
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}
