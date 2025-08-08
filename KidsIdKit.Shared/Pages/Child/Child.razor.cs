using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Shared.Pages.Child;
public partial class Child
{
    [Parameter]
    public int Id { get; set; }
    Data.Child? CurrentChild;
    private string? TemplateString { get; set; }

    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        RemoveEmptyChildRecords();
        if (Id == -1)
        {
            CurrentChild = new Data.Child();
            CurrentChild.ChildDetails.GivenName = string.Empty;
            if (DataStore.Family.Children.Count == 0)
                CurrentChild.Id = 1;
            else
                CurrentChild.Id = DataStore.Family.Children.Max(r => r.Id) + 1;
            DataStore.Family.Children.Add(CurrentChild);
            Id = DataStore.Family.Children.IndexOf(CurrentChild);
            NavigationManager.NavigateTo($"/childDetails/{Id}");
        }
        else if (Id > DataStore.Family.Children.Count - 1)
        {
            NavigationManager.NavigateTo($"/");
        }
        else
        {
            CurrentChild = DataStore.Family.Children[Id];
            SetTemplate();
        }
    }

    private void RemoveEmptyChildRecords()
    {
        for (var i = DataStore.Family!.Children.Count - 1; i >= 1; i--)
        {
            if (string.IsNullOrEmpty(DataStore.Family.Children[i].ChildDetails.GivenName))
                DataStore.Family.Children.RemoveAt(i);
        }
    }

    private RenderFragment ImageContent => builder =>
    {
        builder.OpenElement(0, "img");
        builder.AddAttribute(1, "src", "_content/KidsIdKit.Shared/pdf.jpg");
        builder.AddAttribute(2, "alt", "Generate and Download PDF");
        builder.AddAttribute(3, "style", "width: 30px; height: 30px;");
        builder.AddAttribute(4, "title", "Generate and Download PDF");
        builder.CloseElement();
    };

    async Task GenerateAndDownloadPdf()
    {
        await using var module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../PdfGenerator/HtmlToPdf.js");

        await module.InvokeVoidAsync("generateAndDownloadPdf", TemplateString, $"{CurrentChild!.ChildDetails.GivenName}.pdf");

        // Generate the PDF and get its content as byte[] (need .NET 6 to support Uint8Array)
        var bytes = await module.InvokeAsync<byte[]>("generatePdf", "<h1>sample</h1>");
    }

    private void SetTemplate()
    {
        if (CurrentChild != null)
        {
            var today = DateTime.Now;
            var dayOfWeek = today.DayOfWeek;
            var day = today.ToString("M/d/yyyy");
            var time = today.ToString("HH:mm:ss");
            TemplateString = "<div style='width: 800px; font-family: Arial; line-height: 1.6;'>" +
                              "  <div style='text-align: left;'>" +  // Was "center"-ing
                             $"    <h6>Kids ID Kit info for</h6>" +
                             $"    <h1>{CurrentChild.ChildDetails.GivenName} {CurrentChild.ChildDetails.FamilyName}</h1>" +
                              "  </div>" +
                             $"  <h6>Printed on {dayOfWeek} {day} at {time}</h6>" +
                             $"  {ChildDetails()}" +
                             $"  {PhysicalDetails()}" +
                              "</div>";
        }

        string ChildDetails()
        {
            var ageAndBirthday = $"{CurrentChild.ChildDetails.AgeFormatted} ({CurrentChild.ChildDetails.Birthday.ToString("d")})";
            var childDetails =
           $"    {li("Given name", CurrentChild.ChildDetails.GivenName)}" +
           $"    {li("Nickname", CurrentChild.ChildDetails.NickName)}" +
           $"    {li("Additional name", CurrentChild.ChildDetails.AdditionalName)}" +
           $"    {li("Family name", CurrentChild.ChildDetails.FamilyName)}" +
           $"    {li("Age", ageAndBirthday)} " +
           $"    {li("Phone number", CurrentChild.ChildDetails.PhoneNumber)}" +
           $"    <li style='display: flex; align-items: flex-start; font-weight: bold;'>" +         // Top-align 'Photo:' text with the photo
           $"      <span style='margin-right: 8px;'>Photo:</span>" +
           $"      <img src='{CurrentChild.ChildDetails.Photo.ImageSource}'" +
           $"           title= 'Photo of child'" +
           $"           alt= 'Photo of child'" +
           $"           style='max-height: 150px;' />" +
           $"    </li>";

            return $"{header_div("Child Details", childDetails)}";
        }

        string PhysicalDetails()
        {
            var ageAndBirthday = $"{CurrentChild.ChildDetails.AgeFormatted} ({CurrentChild.ChildDetails.Birthday.ToString("d")})";
            // "  <h2>Physical Details</h2>" +
            return $"{header_div("Physical Details", "")}";

            //  "  <ul>" +
            //  "  <ul style='list-style-type: none;" + // Remove bullets from the list
            //  "             margin: 0;" + // Remove default margin
            //  "             padding: 0;'>" + // Remove default padding
            // $"    {li("Given name", CurrentChild.ChildDetails.GivenName)}" +
            // $"    {li("Nickname", CurrentChild.ChildDetails.NickName)}" +
            // $"    {li("Additional name", CurrentChild.ChildDetails.AdditionalName)}" +
            // $"    {li("Family name", CurrentChild.ChildDetails.FamilyName)}" +
            // $"    {li("Age", ageAndBirthday)} " +
            // $"    {li("Phone number", CurrentChild.ChildDetails.PhoneNumber)}" +
            // $"    <li style='display: flex; align-items: flex-start; font-weight: bold;'>" +         // Top-align 'Photo:' text with the photo
            // $"      <span style='margin-right: 8px;'>Photo:</span>" +
            // $"      <img src='{CurrentChild.ChildDetails.Photo.ImageSource}'" +
            // $"           title= 'Photo of child'" +
            // $"           alt= 'Photo of child'" +
            // $"           style='max-height: 150px;' />" +
            // $"    </li>" +
            //  "  </ul>";

            //             :
            // 08 / 04 / 2025
            // Height:
            //             Hair color:
            // Blond
            // Hair style:
            //             Eye color:
            // Wears contacts:
            // Eye glasses:
            // Skin tone:
            // Racial / ethnic identity:
            //         Sex:
            //             Gender identity:
        }

        string header_div(string header, string divContents)
        {
            return "  <ul style='list-style-type: none;" +   // Remove bullets from the list
                   "             margin: 0;" +               // Remove default margin
                   "             padding: 0;'>" +            // Remove default padding
                  $"    <h2>{header}</h2>" +
                   "    <div style='margin-left: 20px;'>" +  // Indent section contents
                  $"      {divContents}" +
                   "    </div>" +
                   "  </ul>";
                }

        string li(string label, string value)
        {
            // TODO: Question - would the user prefer *nothing* to be output (i.e., not even the label) if this particular data item was not specified/filled in?
            label = $"<span style='font-weight: bold;'>{label}:</span>";
            return "<li>" + label + " " + (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value)
                                                        ? "<span style='font-size: x-small;'>[not specified]</span>"
                                                        : value) +
                   "</li>";
        }
    }
}