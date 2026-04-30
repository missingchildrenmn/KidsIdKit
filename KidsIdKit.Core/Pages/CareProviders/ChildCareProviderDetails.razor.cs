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

    protected override Task OnInitializedAsync()
    {
        var returnValue = base.OnInitializedAsync();

        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (CareId == -1)
            {
                var newCareProvider = new CareProvider();
                newCareProvider.Id = child.ProfessionalCareProviders.Count == 0 ? 0 : child.ProfessionalCareProviders.Max(r => r.Id) + 1;
                PageState.InitStateItem<Data.CareProvider?>(EditingObjectState, newCareProvider);
            }
            else
            {
                var index = child.ProfessionalCareProviders.FindIndex(p => p.Id == CareId);
                if (index >= 0)
                {
                    PageState.InitStateItem<Data.CareProvider?>(EditingObjectState, child.ProfessionalCareProviders[index]);
                }
                else
                {
                    Console.WriteLine($"Care provider with an ID of {CareId} was not found");
                }
            }
            var editingObject = PageState.GetStateItem<Data.CareProvider?>(EditingObjectState).Value;
            if (editingObject != null)
            {
                PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(editingObject));
            }
        }

        return returnValue;
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
            if (index >= 0)
            {
                child.ProfessionalCareProviders[index] = unalteredObject;
            }
            else
            {
                Console.WriteLine($"Care provider with an ID of {CareId} was not found");
            }
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
                var editingObject = PageState.GetStateItem<Data.CareProvider?>(EditingObjectState).Value;
                if (child != null && editingObject is not null)
                {
                    if (CareId == -1)
                    {
                        child.ProfessionalCareProviders.Add(editingObject);
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
