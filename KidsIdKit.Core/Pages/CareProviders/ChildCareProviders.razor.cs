using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.CareProviders;

public partial class ChildCareProviders
{
    [Parameter]
    public int id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<CareProvider>? CareProviders;

    public override string MenuBarTitle { get; protected set; } = "Care Providers";

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this care provider?";
    private const string AlertStateInformationState = "AlertStateInformation";

    private bool ShowBusyIndicator = false;
    private string BusyMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<bool>(AlertShowState, false);
    }

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            CareProviders = child.ProfessionalCareProviders;
        }
    }

    private void NavigateToCareProviderEdit(int childId, int providerId)
    {
        NavigationManager.NavigateTo($"/CareProvider/{childId}/{providerId}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        PageState.SetStateItem(AlertShowState, false);
        int careId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Deleting...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            await Task.Run(async () => { 
                var child = FamilyState.GetChild(id);
                if (child != null && careId >= 0)
                {
                    var careProvider = child.ProfessionalCareProviders.FirstOrDefault((p) => p.Id == careId);
                    if (careProvider is not null)
                    {
                        child.ProfessionalCareProviders.Remove(careProvider);
                        await FamilyState.SaveAsync();
                        CareProviders = child.ProfessionalCareProviders;
                    }
                }
            });
            BusyMessage = string.Empty;
            ShowBusyIndicator = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void ShowAlert(CareProvider careProvider)
    {
        PageState.SetStateItem<string>(AlertTitleState, $"Remove {careProvider.GivenName} {careProvider.FamilyName} ?");
        PageState.SetStateItem(AlertStateInformationState, careProvider.Id.ToString());
        PageState.SetStateItem(AlertShowState, true);
    }
}
