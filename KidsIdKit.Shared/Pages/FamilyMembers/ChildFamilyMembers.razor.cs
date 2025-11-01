using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.FamilyMembers;

public partial class ChildFamilyMembers
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<FamilyMember>? Family;

    readonly string PageTitle = "Family Members";

    protected override void OnParametersSet()
    {
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            Family = DataStore.Family.Children[Id].FamilyMembers;
        }
    }

}