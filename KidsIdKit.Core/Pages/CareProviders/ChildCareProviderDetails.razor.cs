using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.Pages.CareProviders;

public partial class ChildCareProviderDetails : EditablePageBase<Data.CareProvider>
{
    [Parameter]
    public int ChildId { get; set; }

    [Parameter]
    public int CareId { get; set; }

    ChildDetails? CurrentChild;

    readonly string PageTitle = "Care Provider";
    public override string MenuBarTitle { get; protected set; } = "Care Provider";
    protected override void OnInitialized()
    {

        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (CareId == -1)
            {
                EditingObject = new CareProvider();
                EditingObject!.Id = child.ProfessionalCareProviders.Count == 0 ? 0 : child.ProfessionalCareProviders.Max(r => r.Id) + 1;
            }
            else if (CareId >= 0 && CareId < child.ProfessionalCareProviders.Count)
            {
                EditingObject = child.ProfessionalCareProviders[CareId];
            }
            originalSnapshot = SerializeObject(EditingObject!);
        }
    }

    protected override CareProvider ResetUnalteredObject(CareProvider unalteredObject)
    {
        var child = FamilyState.GetChild(ChildId);
        if (child == null)
        {
            return unalteredObject;
        }

        if (child.ProfessionalCareProviders.Any(f => f.Id == CareId))
        {
            var index = child.ProfessionalCareProviders.FindIndex(f => f.Id == unalteredObject.Id);
            child.ProfessionalCareProviders[index] = unalteredObject;
        }
        return unalteredObject;
    }

    protected override async Task SaveData()
    {
        if (ValidateChangesForSave())
        {
            try
            {
                var child = FamilyState.GetChild(ChildId);
                if (child != null && EditingObject is not null)
                {
                    if (CareId == -1)
                    {
                        child.ProfessionalCareProviders.Add(EditingObject);
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
}
