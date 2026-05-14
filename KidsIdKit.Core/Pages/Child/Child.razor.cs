using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.Pages.Child;

public partial class Child
{
    [Inject] private IChildPdfRenderer PdfRenderer { get; set; } = default!;

    [Parameter] public int Id { get; set; }

    Data.Child? CurrentChild;
    private byte[]? PdfBytes { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Child Information";

    protected bool ShowBusyIndicator = false;
    protected string BusyMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await FamilyState.LoadAsync();

        if (FamilyState.Family == null)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        RemoveEmptyChildRecords();

        if (Id == -1)
        {
            CurrentChild = new Data.Child();
            CurrentChild.ChildDetails.GivenName = string.Empty;
            CurrentChild.Id = FamilyState.Family.Children.Count == 0 
                ? 1 
                : FamilyState.Family.Children.Max(r => r.Id) + 1;
            FamilyState.Family.Children.Add(CurrentChild);
            Id = FamilyState.Family.Children.IndexOf(CurrentChild);
            await NavigateForNewChild(Id);
        }
        else if (Id < 0 || Id >= FamilyState.Family.Children.Count)
        {
            NavigationManager.NavigateTo("/");
        }
        else
        {
            CurrentChild = FamilyState.Family.Children[Id];
        }
    }

    private void RemoveEmptyChildRecords()
    {
        if (FamilyState.Family == null) return;

        for (var i = FamilyState.Family.Children.Count - 1; i >= 1; i--)
        {
            if (string.IsNullOrEmpty(FamilyState.Family.Children[i].ChildDetails.GivenName))
                FamilyState.Family.Children.RemoveAt(i);
        }
    }

    private async Task SendAllInfo()
    {
        try
        {
            Console.WriteLine("SendAllInfo: Method called");

            if (CurrentChild == null)
            {
                Console.WriteLine("SendAllInfo: CurrentChild is null");
                return;
            }

            if (PdfBytes == null || PdfBytes.Length == 0)
            {
                Console.WriteLine("SendAllInfo: PdfBytes is null/empty, regenerating...");
                BusyMessage = "Generating PDF...";
                ShowBusyIndicator = true;
                await InvokeAsync(StateHasChanged);
                try
                {
                    await Task.Run(() =>
                    {
                        PdfBytes = PdfRenderer.RenderChildToPdf(CurrentChild);
                    });
                }
                finally
                {
                    BusyMessage = string.Empty;
                    ShowBusyIndicator = false;
                    await InvokeAsync(StateHasChanged);
                }
            }

            if (PdfBytes == null || PdfBytes.Length == 0)
            {
                Console.WriteLine("SendAllInfo: PdfBytes is still null/empty after regeneration");
                return;
            }

            var filename = $"{(CurrentChild.ChildDetails.FullName?.Replace(' ', '-')) ?? "unknown-child"}.pdf";

            Console.WriteLine($"SendAllInfo: Starting to save file '{filename}' with {PdfBytes.Length} bytes");

            var saveResult = await FileSaverService.SaveFileAsync(filename, PdfBytes);
            Console.WriteLine($"SendAllInfo: Save result: {saveResult}");

            if (saveResult)
            {
                Console.WriteLine($"SendAllInfo: File saved successfully, now sharing '{filename}'");
                await FileSharerService.ShareFileAsync(filename);
                Console.WriteLine($"SendAllInfo: Process completed for '{filename}'");
            }
            else
            {
                Console.WriteLine($"SendAllInfo: Failed to save file '{filename}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SendAllInfo: Error occurred: {ex.Message}");
            Console.WriteLine($"SendAllInfo: Stack trace: {ex.StackTrace}");
        }
    }
    private async Task NavigateForNewChild(int id)
    {
        NavigationManager.NavigateTo($"/childDetails/{id}", false, true);
    }
}
