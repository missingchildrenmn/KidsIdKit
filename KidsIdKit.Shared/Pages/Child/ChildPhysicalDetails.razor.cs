using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.Child;

public partial class ChildPhysicalDetails
{
    [Parameter]
    public int Id { get; set; }

    Data.ChildDetails? CurrentChild;
    Data.PhysicalDetails? Details;

    string PageTitle = "Physical Details";

    protected override void OnParametersSet()
    {
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            Details = DataStore.Family.Children[Id].PhysicalDetails;
        }
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}