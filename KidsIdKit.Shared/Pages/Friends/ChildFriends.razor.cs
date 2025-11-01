using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.Friends;

public partial class ChildFriends
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<Person>? Friends;

    readonly string PageTitle = "Child's Friends";

    protected override void OnParametersSet()
    {
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            Friends = DataStore.Family.Children[Id].Friends;
        }
    }
}