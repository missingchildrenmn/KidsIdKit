using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.CareProviders;

public partial class ChildCareProviders
{
    [Parameter]
    public int id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<CareProvider>? CareProviders;

    readonly string PageTitle = "Care Providers";

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            CareProviders = child.ProfessionalCareProviders;
        }
    }
}
