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

    readonly string PageTitle = "Care Providers";
    public override string MenuBarTitle { get; protected set; } = "Care Providers";

    private bool AlertShow = false;
    private string AlertTitle = string.Empty;
    private string AlertMessage = "Are you sure you want to remove this care provider?";
    private string AlertStateInformation = string.Empty;

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
        AlertShow = false;
        int careId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
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
        }
    }

    public void ShowAlert(CareProvider careProvider)
    {
        AlertTitle = $"Remove {careProvider.GivenName} {careProvider.FamilyName} ?";
        AlertStateInformation = careProvider.Id.ToString();
        AlertShow = true;
    }
}
