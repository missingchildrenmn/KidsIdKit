﻿using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Data {
  public class ChildDetails {
    [Required]
    [Display(Name="Given name")]
    public string? GivenName { get; set; }
    [Display(Name = "Nickname")]
    public string? NickName { get; set; }
    [Display(Name = "Additional name")]
    public string? AdditionalName { get; set; }
    [Display(Name = "Family name")]
    public string? FamilyName { get; set; }
    public DateTime Birthday { get; set; } = DateTime.Today;
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }
  }
}
