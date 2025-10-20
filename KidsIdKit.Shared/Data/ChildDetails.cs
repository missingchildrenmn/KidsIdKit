using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace KidsIdKit.Shared.Data;

public class ChildDetails 
{
    #region Names
    [Required]
    [Display(Name = "Given name")]
    public string? GivenName { get; set; }

    [Display(Name = "Nickname")]
    public string? NickName { get; set; }

    [Display(Name = "Additional name")]
    public string? AdditionalName { get; set; }

    public string Names // First names (including aliases?)
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

    public string? FullName
    {
        get
        {
            var names = new[] { GivenName?.Trim(), FamilyName?.Trim() }
                .Where(name => !string.IsNullOrWhiteSpace(name));
            return string.Join(" ", names);
        }
    }

    public int Age { get => DateTime.Today.Year - Birthday.Year; }
    #endregion

    public DateTime Birthday { get; set; } = DateTime.Today;
    public string AgeFormatted => Format(Birthday);

    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Photo")]
    public Photo Photo { get; set; } = new();

    public string Format(DateTime birthDate)
    {
        var today = DateTime.Today;
        var ageSpan = today - birthDate;

        var roughEstimateOfAgeInYears = RoughEstimateOfAgeInYears(ageSpan);
        if (roughEstimateOfAgeInYears < 3)
        {
            return ageSpan.Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Month, precision: 2);
        }

        return $"{roughEstimateOfAgeInYears} year{(roughEstimateOfAgeInYears > 1 ? "s" : string.Empty)} old";
    }
 
    private static int RoughEstimateOfAgeInYears(TimeSpan ageSpan)
    {
        var averageNumberOfDaysInAYearAccountingForLeapYears = 365.25;
        return (int)(ageSpan.TotalDays / averageNumberOfDaysInAYearAccountingForLeapYears);
    }
}
