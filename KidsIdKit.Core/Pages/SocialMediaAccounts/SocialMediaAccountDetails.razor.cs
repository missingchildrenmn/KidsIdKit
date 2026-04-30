using KidsIdKit.Core.Data;
using KidsIdKit.Core.Pages.Child;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

public partial class SocialMediaAccountDetails : EditablePageBase<Data.SocialMediaAccount>
{
    [Inject] private IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public int ChildId { get; set; }
    [Parameter] public int SocialMediaAccountId { get; set; }

    Data.ChildDetails? CurrentChild;

    private string? messageText;
    // TODO: Extract "Social Media Account" from .razor file to a PageTitle field
    public override string MenuBarTitle { get; protected set; } = "Social Media";


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (SocialMediaAccountId == -1)
            {
                var newAccount = new SocialMediaAccount();
                newAccount.Id = child.SocialMediaAccounts.Count == 0 ? 0 : child.SocialMediaAccounts.Max(r => r.Id) + 1;
                PageState.InitStateItem<Data.SocialMediaAccount?>(EditingObjectState, newAccount);
            }
            else if (SocialMediaAccountId >= 0 && SocialMediaAccountId < child.SocialMediaAccounts.Count)
            {
                var index = child.SocialMediaAccounts.FindIndex(f => f.Id == SocialMediaAccountId);
                if (index >= 0)
                {
                    PageState.InitStateItem<Data.SocialMediaAccount?>(EditingObjectState, child.SocialMediaAccounts[index]);
                }
                else
                {
                    Console.WriteLine($"Social media account with an ID of {SocialMediaAccountId} was not found.");
                }
            }

            var editingObject = PageState.GetStateItem<Data.SocialMediaAccount?>(EditingObjectState).Value;
            if (editingObject != null)
            {
                PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(editingObject));
            }
        }
    }

    protected override SocialMediaAccount ResetUnalteredObject(SocialMediaAccount unalteredObject)
    {
        var child = FamilyState.GetChild(ChildId);
        if (child == null)
        {
            return unalteredObject;
        }

        if (child.SocialMediaAccounts.Any(f => f.Id == SocialMediaAccountId))
        {
            var index = child.SocialMediaAccounts.FindIndex(f => f.Id == unalteredObject.Id);
            child.SocialMediaAccounts[index] = unalteredObject;
        }
        return unalteredObject;
    }

    protected override async Task SaveData()
    {
        messageText = string.Empty;
        if (ValidateChangesForSave())
        {
            try
            {
                var child = FamilyState.GetChild(ChildId);
                var editingObject = PageState.GetStateItem<Data.SocialMediaAccount?>(EditingObjectState).Value;
                if (child != null && editingObject is not null)
                {
                    if (SocialMediaAccountId == -1)
                    {
                        child.SocialMediaAccounts.Add(editingObject);
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
}
