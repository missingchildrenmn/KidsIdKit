using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildDetails
{
    [Parameter] public int Id { get; set; }

    Data.ChildDetails? CurrentChild { get; set; }

    readonly string PageTitle = "Child Details";
    private bool SelectingImage;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        CurrentChild = child?.ChildDetails;
    }

    private async Task NavigateBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private void DateChanged(string newDate)
    {
        var a = newDate;
        Console.WriteLine(newDate);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            Console.WriteLine(assembly.FullName);
        }

        if (firstRender)
        {
            var objRef = DotNetObjectReference.Create(this);
            JSRuntime.InvokeVoidAsync("setDateEventhandler", objRef);
        }
    }

    [JSInvokable("UpdateBirthday")]
    public void UpdateBirthday(string newValue)
    {
        CurrentChild?.Birthday = DateTime.Parse(newValue);
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");

}
