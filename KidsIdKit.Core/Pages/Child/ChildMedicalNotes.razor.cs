using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildMedicalNotes
{
    [Parameter]
    public int Id { get; set; }
    Data.ChildDetails? CurrentChild;
    Data.MedicalNotes? MedicalNotes;

    readonly string PageTitle = "Medical notes";

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            MedicalNotes = child.MedicalNotes;
        }
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}
