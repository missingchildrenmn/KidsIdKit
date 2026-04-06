
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Information;

public partial class Safety
{
    public override string MenuBarTitle { get; protected set; } = "Safety";
    private MarkupString AgeRange(string firstAge, string secondAge)
    {
        return (MarkupString)$"(ages {firstAge} {@InfoConstants.EN_DASH} {secondAge})";     // '(ages x - y)'
    }

    // private MarkupString EmergencyPhoneNumberLink()
    // {
    //     var doubleQuote = InfoConstants.DOUBLE_QUOTE;
    //     var emergencyPhoneNumber = "911";
    //     return (MarkupString)$"<a href={doubleQuote}tel:{emergencyPhoneNumber}{doubleQuote}>{emergencyPhoneNumber}</a>";
    // }
    // //     <li>How to dial @EmergencyPhoneNumberLink() in an emergency</li>

}
