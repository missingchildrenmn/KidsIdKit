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
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        if (Id >= 0 && Id < DataStore.Family.Children.Count)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            MedicalNotes = DataStore.Family.Children[Id].MedicalNotes;
        }
    }

    private async Task SaveData() => await SaveData($"/child/{Id}");
}
