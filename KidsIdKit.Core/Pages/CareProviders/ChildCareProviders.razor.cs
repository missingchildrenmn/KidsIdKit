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
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[id].ChildDetails;
            CareProviders = DataStore.Family.Children[id].ProfessionalCareProviders;
        }
    }
}
