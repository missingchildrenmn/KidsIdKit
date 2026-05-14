using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatures
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IQueryable<DistinguishingFeature>? Features { get; set; }

    private bool ShowBusyIndicator = false;
    private string BusyMessage = string.Empty;

    public override string MenuBarTitle { get; protected set; } = "Distinguishing Features";

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this distinguishing feature?";
    private const string AlertStateInformationState = "AlertStateInformation";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageState.InitStateItem<bool>(AlertShowState, false);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
    }

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Features = child.DistinguishingFeatures.AsQueryable();
        }
    }

    private void NavigateToEdit(DistinguishingFeature feature)
    {
        NavigationManager.NavigateTo($"/Feature/{Id}/{feature.Id}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        PageState.SetStateItem<bool>(AlertShowState, false);
        int distinguishingFeatureId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Deleting...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            await Task.Run(async () => {
                var child = FamilyState.GetChild(Id);
                if (child != null && distinguishingFeatureId >= 0)
                {
                    var distinguishingFeature = child.DistinguishingFeatures.FirstOrDefault((p) => p.Id == distinguishingFeatureId);
                    if (distinguishingFeature is not null)
                    {
                        child.DistinguishingFeatures.Remove(distinguishingFeature);
                        await FamilyState.SaveAsync();
                        Features = child.DistinguishingFeatures.AsQueryable();
                    }
                }
            });
            BusyMessage = string.Empty;
            ShowBusyIndicator = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void ShowAlert(DistinguishingFeature distinguishingFeature)
    {
        PageState.SetStateItem<string>(AlertStateInformationState, distinguishingFeature.Id.ToString());
        PageState.SetStateItem<string>(AlertTitleState, $"Remove {distinguishingFeature.Description} ?");
        PageState.SetStateItem<bool>(AlertShowState, true);
    }
}
