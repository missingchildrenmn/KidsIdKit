using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.Pages.CareProviders;

public partial class ChildCareProviderDetails
{
    [Parameter]
    public int childId { get; set; }

    [Parameter]
    public int careId { get; set; }

    ChildDetails? CurrentChild;
    CareProvider? CareProvider;

    readonly string PageTitle = "Care Provider";
    public override string MenuBarTitle { get; protected set; } = "Care Provider";
    protected override void OnInitialized()
    {

        var child = FamilyState.GetChild(childId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (careId == -1)
            {
                CareProvider = new CareProvider();
                CareProvider.Id = child.ProfessionalCareProviders.Count == 0 ? 0 : child.ProfessionalCareProviders.Max(r => r.Id) + 1;
            }
            else if (careId >= 0 && careId < child.ProfessionalCareProviders.Count)
            {
                CareProvider = child.ProfessionalCareProviders[careId];
            }
        }
    }

    private async Task SaveData()
    {
        try
        {
            var child = FamilyState.GetChild(childId);
            if (child != null && CareProvider is not null)
            {
                if (careId == -1)
                {
                    child.ProfessionalCareProviders.Add(CareProvider);
                }
                await FamilyState.SaveAsync();
            }
            await NavigateBack();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
