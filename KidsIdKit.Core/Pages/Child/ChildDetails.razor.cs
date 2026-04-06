using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildDetails
{
    [Parameter] public int Id { get; set; }

    Data.ChildDetails? CurrentChild { get; set; }

    public override string MenuBarTitle { get; protected set; } = string.Empty;

    readonly string PageTitle = "Child Details";
    private bool SelectingImage;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        CurrentChild = child?.ChildDetails;
        MenuBarTitle = CurrentChild == null ? PageTitle : string.IsNullOrWhiteSpace(CurrentChild.GivenName) ? "New Child" : PageTitle;
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}
