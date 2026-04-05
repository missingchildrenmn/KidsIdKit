using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.Pages.Information;

public partial class Amberalert
{

    public override string MenuBarTitle { get; protected set; } = "AMBER Alerts";

    private string NationalCrimeInformationCenterUrl()
    {
        return "https://www.ojp.gov/ncjrs/virtual-library/abstracts/national-crime-information-center-ncic-investigative-tool-guide-use";
    }
}
