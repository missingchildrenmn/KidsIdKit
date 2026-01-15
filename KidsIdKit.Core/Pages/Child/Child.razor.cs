using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class Child
{
    [Inject] private IChildHtmlRenderer HtmlRenderer { get; set; } = default!;

    [Parameter] public int Id { get; set; }

    Data.Child? CurrentChild;
    private string? TemplateString { get; set; }

    protected override async Task OnInitializedAsync()
    {
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
            if (FamilyState.Family.Children.Count == 0)
                CurrentChild.Id = 1;
            else
                CurrentChild.Id = FamilyState.Family.Children.Max(r => r.Id) + 1;
            FamilyState.Family.Children.Add(CurrentChild);
            Id = FamilyState.Family.Children.IndexOf(CurrentChild);
            NavigationManager.NavigateTo($"/childDetails/{Id}");
        }
        else if (Id < 0 || Id >= FamilyState.Family.Children.Count)
        {
            NavigationManager.NavigateTo("/");
        }
        else
        {
            CurrentChild = FamilyState.Family.Children[Id];
            TemplateString = HtmlRenderer.RenderChildToHtml(CurrentChild);
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

            if (string.IsNullOrEmpty(TemplateString))
            {
                Console.WriteLine("SendAllInfo: TemplateString is null/empty, regenerating...");
                TemplateString = HtmlRenderer.RenderChildToHtml(CurrentChild);
            }

            if (string.IsNullOrEmpty(TemplateString))
            {
                Console.WriteLine("SendAllInfo: TemplateString is still null/empty after regeneration");
                return;
            }

            var filename = $"{(CurrentChild.ChildDetails.FullName?.Replace(' ', '-')) ?? "unknown-child"}.html";

            Console.WriteLine($"SendAllInfo: Starting to save file '{filename}' with content length {TemplateString.Length}");

            var saveResult = await FileSaverService.SaveFileAsync(filename, TemplateString);
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
}
