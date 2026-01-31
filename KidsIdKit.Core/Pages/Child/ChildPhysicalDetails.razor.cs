using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildPhysicalDetails
{
    [Parameter]
    public int Id { get; set; }

    Data.ChildDetails? CurrentChild;
    Data.PhysicalDetails? Details;

    readonly string PageTitle = "Physical Details";

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Details = child.PhysicalDetails;
        }
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}
