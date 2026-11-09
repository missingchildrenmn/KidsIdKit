using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using static KidsIdKit.Core.SharedComponents.RemovalConfirmation;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.CareProviders;

public partial class ChildCareProviders
{
    [Parameter]
    public int id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<CareProvider>? CareProviders;

    public override string MenuBarTitle { get; protected set; } = "Care Providers";

    private bool ShowBusyIndicator = false;
    private string BusyMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<bool>(AlertShowState, false);
        PageState.InitStateItem<string>(AlertMessageState, string.Empty);
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
            try
            {
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
            }
            finally
            {
                BusyMessage = string.Empty;
                ShowBusyIndicator = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public void ShowAlert(CareProvider careProvider)
    {
        PageState.SetStateItem<string>(AlertTitleState, RemovalConfirmation.Header);
        PageState.SetStateItem(AlertStateInformationState, careProvider.Id.ToString());
        PageState.SetStateItem<string>(AlertMessageState, RemovalConfirmation.FormatMessage($"{careProvider.GivenName} {careProvider.FamilyName}"));
        PageState.SetStateItem(AlertShowState, true);
    }
}
