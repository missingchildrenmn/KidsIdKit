using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildPhysicalDetails
{
    [Parameter]
    public int Id { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Physical Details";

    Data.ChildDetails? CurrentChild;
    Data.PhysicalDetails? Details;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Details = child.PhysicalDetails;
        }
    }

    private async Task SaveData() => await InternalSaveData();
}
