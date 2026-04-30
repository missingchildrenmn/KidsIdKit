using KidsIdKit.Core.Data;
using KidsIdKit.Core.Pages.SocialMediaAccounts;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatureDetails : EditablePageBase<Data.DistinguishingFeature>
{
    [Inject] private IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public int ChildId { get; set; }
    [Parameter] public int FeatureId { get; set; }

    readonly string PageTitle = "Distinguishing Feature (birthmark, scar, etc.)";
    KidsIdKit.Core.Data.ChildDetails? CurrentChild;
    //DistinguishingFeature? DistinguishingFeature;
    private bool SelectingImage;
    private string? messageText;
    public override string MenuBarTitle { get; protected set; } = "Distinguishing Feature";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FeatureId == -1)
            {
                var newFeature = new DistinguishingFeature();
                newFeature.Id = child.DistinguishingFeatures.Count == 0 ? 0 : child.DistinguishingFeatures.Max(r => r.Id) + 1;
                PageState.InitStateItem<Data.DistinguishingFeature?>(EditingObjectState, newFeature);
            }
            else if (FeatureId >= 0 && FeatureId < child.DistinguishingFeatures.Count)
            {
                PageState.InitStateItem<Data.DistinguishingFeature?>(EditingObjectState, child.DistinguishingFeatures[FeatureId]);
            }
            var editingObject = PageState.GetStateItem<Data.DistinguishingFeature?>(EditingObjectState).Value;
            if (editingObject != null)
            {
                PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(editingObject));
            }
        }
    }

    protected override async Task SaveData()
    {
        messageText = string.Empty;
        // Validate before saving
        if (ValidateChangesForSave())
        {
            try
            {
                var child = FamilyState.GetChild(ChildId);
                var editingObject = PageState.GetStateItem<Data.DistinguishingFeature?>(EditingObjectState).Value;
                if (child != null && editingObject is not null)
                {
                    if (FeatureId == -1)
                    {
                        child.DistinguishingFeatures.Add(editingObject);
                    }
                    await FamilyState.SaveAsync();
                }
                await NavigateBack();
            }
            catch (Exception e)
            {
                messageText = e.Message;
            }
        }
    }

    protected override DistinguishingFeature ResetUnalteredObject(DistinguishingFeature unalteredObject)
    {
        var child = FamilyState.GetChild(ChildId);
        if (child == null)
        {
            return unalteredObject;
        }

        if (child.DistinguishingFeatures.Any(f => f.Id == FeatureId))
        {
            var index = child.DistinguishingFeatures.FindIndex(f => f.Id == FeatureId);
            if (index >= 0)
            {
                child.DistinguishingFeatures[index] = unalteredObject;
            }
            else
            {
                Console.WriteLine($"Distinguishing Feature with an ID of {FeatureId} was not found.");
            }
        }
        return unalteredObject;
    }
}
