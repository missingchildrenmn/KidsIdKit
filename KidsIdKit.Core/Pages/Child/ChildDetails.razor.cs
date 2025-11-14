using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildDetails
{
    [Parameter] public int Id { get; set; }

    Data.ChildDetails? CurrentChild { get; set; }

    readonly string PageTitle = "Child Details";
    private bool SelectingImage;

    protected override void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        CurrentChild = DataStore.Family.Children[Id].ChildDetails;
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}
