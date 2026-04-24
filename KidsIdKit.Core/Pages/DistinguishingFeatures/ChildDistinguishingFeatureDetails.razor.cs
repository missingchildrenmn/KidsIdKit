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
    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FeatureId == -1)
            {
                EditingObject = new DistinguishingFeature();
                EditingObject!.Id = child.DistinguishingFeatures.Count == 0 ? 0 : child.DistinguishingFeatures.Max(r => r.Id) + 1;
            }
            else if (FeatureId >= 0 && FeatureId < child.DistinguishingFeatures.Count)
            {
                EditingObject = child.DistinguishingFeatures[FeatureId];
            }
            originalSnapshot = SerializeObject(EditingObject!);
        }
        ShowPendingChangesAlert = false;
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
                if (child != null && EditingObject is not null)
                {
                    if (FeatureId == -1)
                    {
                        child.DistinguishingFeatures.Add(EditingObject);
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
