using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Information;

public partial class Abductions
{
    private string minnesotaStateStatuteText = "MN State Statute";
    private string amecoPhoneNumber = "(877) 263-2620";
    private MarkupString? phoneLink;
    private string minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl = "https://www.revisor.mn.gov/statutes/";

    private bool ExpandMinnesotaSection = true; // This can be toggled to show/hide the Minnesota section

    public override string MenuBarTitle { get; protected set; } = "Abductions";

    private void ToggleMinnesotaSection() => ExpandMinnesotaSection = !ExpandMinnesotaSection;

    // private bool ExpandOutsideMinnesotaSection = true; // This can be toggled to show/hide the outside Minnesota section

    protected override void OnInitialized()
    {
        phoneLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink(amecoPhoneNumber);
    }

    private string MinnesotaStateStatuteDetailsAreHere(string minnesotaStateStatute)                 // Details of MN State Statute nnn.nn[n] can be found here (where 'here' is a hyperlink)
    {
        var minnesotaLegislatureOfficeOfTheRevisorOfStatutesDetailsUrl = MinnesotaStateStatuteDetailsUrl(minnesotaStateStatute);
        var hereLink = HyperlinkHelper.LinkHelper.Link("here", minnesotaLegislatureOfficeOfTheRevisorOfStatutesDetailsUrl);
        var minnesotaLegislatureOfficeOfTheRevisorOfStatutesDetailsUrlWithId = $"Details of {MinnesotaStateStatute(minnesotaStateStatute)} can be found {hereLink}";
        return minnesotaLegislatureOfficeOfTheRevisorOfStatutesDetailsUrlWithId;
    }

    private string MinnesotaStateStatuteDetailsUrl(string minnesotaStateStatute)                     // https://www.revisor.mn.gov/statutes/?id=nnn.nn[n]
    {
        return $"{minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl}?id={minnesotaStateStatute}";
    }

    private string MinnesotaStateStatute(string minnesotaStateStatute)                               // MN State Statute nnn.nn[n]
    {
        return $"{this.minnesotaStateStatuteText} {minnesotaStateStatute}";
    }

    private MarkupString MinnesotaStateStatuteHyperlink(string minnesotaStateStatute)                // 'MN State Statute 609.26' hyperlink
    {
        return (MarkupString)HyperlinkHelper.LinkHelper.Link(MinnesotaStateStatute(minnesotaStateStatute), MinnesotaStateStatuteDetailsUrl(minnesotaStateStatute));
    }
}