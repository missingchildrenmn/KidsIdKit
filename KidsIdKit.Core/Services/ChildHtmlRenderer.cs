using System.Text;
using KidsIdKit.Core.Data;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Renders child data to HTML format for display and export.
/// </summary>
public class ChildHtmlRenderer : IChildHtmlRenderer
{
    private const string NoneSpecified = "[none specified]";
    private const string NotSpecified = "[not specified]";

    /// <inheritdoc />
    public string RenderChildToHtml(Child child)
    {
        ArgumentNullException.ThrowIfNull(child);

        var today = DateTime.Now;
        var dayOfWeek = today.DayOfWeek;
        var day = today.ToString("M/d/yyyy");
        var time = today.ToString("HH:mm:ss");

        return "<div style='width: 800px; font-family: Arial; line-height: 1.6;'>" +
               "  <div style='text-align: left;'>" +
               $"    <h6>Kids ID Kit information for</h6>" +
               $"    <h1>{child.ChildDetails.FullName}</h1>" +
               "  </div>" +
               $"  <h6>Printed on {dayOfWeek} {day} at {time}</h6>" +
               $"  {RenderChildDetails(child)}" +
               $"  {RenderPhysicalDetails(child)}" +
               string.Concat(Enumerable.Repeat("<br />", 2)) +
               $"  {RenderDistinguishingFeatures(child)}" +
               $"  {RenderFamilyMembers(child)}" +
               $"  {RenderFriends(child)}" +
               $"  {RenderCareProviders(child)}" +
               $"  {RenderMedicalNotes(child)}" +
               "</div>";
    }

    private static string RenderChildDetails(Child child)
    {
        var ageAndBirthday = $"{child.ChildDetails.AgeFormatted} (born {child.ChildDetails.Birthday.ToString("d")})";
        var childDetails =
            $"    {RenderListItem("Given name", child.ChildDetails.GivenName ?? "")}" +
            $"    {RenderListItem("Nickname", child.ChildDetails.NickName ?? "")}" +
            $"    {RenderListItem("Additional name", child.ChildDetails.AdditionalName ?? "")}" +
            $"    {RenderListItem("Family name", child.ChildDetails.FamilyName ?? "")}" +
            $"    {RenderListItem("Age", ageAndBirthday)} " +
            $"    {RenderListItem("Phone number", child.ChildDetails.PhoneNumber ?? "")}" +
            $"    <li style='display: flex; align-items: flex-start; font-weight: bold;'>" +
            $"      <span style='margin-right: 8px;'>Photo:</span>" +
            $"      <img src='{child.ChildDetails.Photo.ImageSource}'" +
            $"           title= 'Photo of child'" +
            $"           alt= 'Photo of child'" +
            $"           style='max-height: 150px;' />" +
            $"    </li>";

        return RenderSection("Child Details", childDetails);
    }

    private static string RenderPhysicalDetails(Child child)
    {
        var physicalDetails =
            $"    {RenderListItem("Measurement date", child.PhysicalDetails.MeasurementDate.ToString("d"))}" +
            $"    {RenderListItem("Height", child.PhysicalDetails.Height ?? "")}" +
            $"    {RenderListItem("Hair color", child.PhysicalDetails.HairColor ?? "")}" +
            $"    {RenderListItem("Hair style", child.PhysicalDetails.HairStyle ?? "")}" +
            $"    {RenderListItem("Eye color", child.PhysicalDetails.EyeColor ?? "")} " +
            $"    {RenderListItem("Wears contacts", child.PhysicalDetails.EyeContacts.ToString())}" +
            $"    {RenderListItem("Eye glasses", child.PhysicalDetails.EyeGlasses.ToString())}" +
            $"    {RenderListItem("Skin tone", child.PhysicalDetails.SkinTone ?? "")}" +
            $"    {RenderListItem("Racial / ethnic identity", child.PhysicalDetails.RacialEthnicIdentity ?? "")}" +
            $"    {RenderListItem("Sex", child.PhysicalDetails.Sex ?? "")}" +
            $"    {RenderListItem("Gender identity", child.PhysicalDetails.GenderIdentity ?? "")}";

        return RenderSection("Physical Details", physicalDetails);
    }

    private static string RenderDistinguishingFeatures(Child child)
    {
        var features = child.DistinguishingFeatures;
        string content;

        if (features.Count == 0)
        {
            content = NoneSpecified;
        }
        else
        {
            var rows = new StringBuilder();
            foreach (var feature in features)
            {
                var description = feature.Description ?? NotSpecified;
                var photoHtml = feature.Photo?.ImageSource == null
                    ? NotSpecified
                    : $"<img src='{feature.Photo.ImageSource}' title='Photo of Distinguishing feature' alt='Photo of Distinguishing feature' style='max-height: 150px;' />";
                rows.Append("<tr>")
                    .Append($"  <td>{description}</td>")
                    .Append($"  <td>{photoHtml}</td>")
                    .Append("</tr>");
            }

            content =
                " <table style='width: 100%'>" +
                "  <tr>" +
                "    <th style='width: 30%;'>Description</th>" +
                "    <th>Photo</th>" +
                "  </tr>" +
                $"    {rows}" +
                "</table>";
        }

        return RenderSection("Distinguishing Features", content);
    }

    private static string RenderFamilyMembers(Child child)
    {
        var familyMembers = child.FamilyMembers;
        string content;

        if (familyMembers.Count == 0)
        {
            content = NoneSpecified;
        }
        else
        {
            var rows = new StringBuilder();
            foreach (var member in familyMembers)
            {
                rows.Append("<tr>")
                    .Append($"  <td>{member.GivenName ?? NotSpecified}</td>")
                    .Append($"  <td>{member.NickName ?? NotSpecified}</td>")
                    .Append($"  <td>{member.FamilyName ?? NotSpecified}</td>")
                    .Append($"  <td>{member.Relation ?? NotSpecified}</td>")
                    .Append($"  <td>{member.Address ?? NotSpecified}</td>")
                    .Append($"  <td>{member.PhoneNumber ?? NotSpecified}</td>")
                    .Append("</tr>");
            }

            content =
                " <table style='width: 100%'>" +
                "  <tr>" +
                "    <th style='width: 30%;'>Given Name</th>" +
                "    <th>Nickname</th>" +
                "    <th>Family Name</th>" +
                "    <th>Relation</th>" +
                "    <th>Address</th>" +
                "    <th>Phone Number</th>" +
                "  </tr>" +
                $"    {rows}" +
                "</table>";
        }

        return RenderSection("Family Members", content);
    }

    private static string RenderFriends(Child child)
    {
        var friends = child.Friends;
        string content;

        if (friends.Count == 0)
        {
            content = NoneSpecified;
        }
        else
        {
            var rows = new StringBuilder();
            foreach (var friend in friends)
            {
                rows.Append("<tr>")
                    .Append($"  <td>{friend.GivenName ?? NotSpecified}</td>")
                    .Append($"  <td>{friend.NickName ?? NotSpecified}</td>")
                    .Append($"  <td>{friend.FamilyName ?? NotSpecified}</td>")
                    .Append($"  <td>{friend.Address ?? NotSpecified}</td>")
                    .Append($"  <td>{friend.PhoneNumber ?? NotSpecified}</td>")
                    .Append("</tr>");
            }

            content =
                " <table style='width: 100%'>" +
                "  <tr>" +
                "    <th style='width: 30%;'>Given Name</th>" +
                "    <th>Nickname</th>" +
                "    <th>Family Name</th>" +
                "    <th>Address</th>" +
                "    <th>Phone Number</th>" +
                "  </tr>" +
                $"    {rows}" +
                "</table>";
        }

        return RenderSection("Friends", content);
    }

    private static string RenderCareProviders(Child child)
    {
        var careProviders = child.ProfessionalCareProviders;
        string content;

        if (careProviders.Count == 0)
        {
            content = NoneSpecified;
        }
        else
        {
            var rows = new StringBuilder();
            foreach (var provider in careProviders)
            {
                rows.Append("<tr>")
                    .Append($"  <td>{provider.ClinicName ?? NotSpecified}</td>")
                    .Append($"  <td>{provider.GivenName ?? NotSpecified}</td>")
                    .Append($"  <td>{provider.FamilyName ?? NotSpecified}</td>")
                    .Append($"  <td>{provider.CareRoleDescription ?? NotSpecified}</td>")
                    .Append($"  <td>{provider.PhoneNumber ?? NotSpecified}</td>")
                    .Append($"  <td>{provider.Address ?? NotSpecified}</td>")
                    .Append("</tr>");
            }

            content =
                " <table style='width: 100%'>" +
                "  <tr>" +
                "    <th style='width: 30%;'>Clinic Name</th>" +
                "    <th>Given Name</th>" +
                "    <th>Family Name</th>" +
                "    <th>Role</th>" +
                "    <th>Phone Number</th>" +
                "    <th>Address</th>" +
                "  </tr>" +
                $"    {rows}" +
                "</table>";
        }

        return RenderSection("Care Providers", content);
    }

    private static string RenderMedicalNotes(Child child)
    {
        var medicalNotes =
            $"    {RenderListItem("MedicAlertInfo", child.MedicalNotes.MedicAlertInfo ?? "")}" +
            $"    {RenderListItem("Allergies", child.MedicalNotes.Allergies ?? "")}" +
            $"    {RenderListItem("RegularMedications", child.MedicalNotes.RegularMedications ?? "")}" +
            $"    {RenderListItem("Psychiatric Medications", child.MedicalNotes.PsychMedications ?? "")}" +
            $"    {RenderListItem("Notes", child.MedicalNotes.Notes ?? "")} " +
            $"    {RenderListItem("Inhaler", child.MedicalNotes.Inhaler.ToString())}" +
            $"    {RenderListItem("Diabetic", child.MedicalNotes.Diabetic.ToString())}";

        return RenderSection("Medical Notes", medicalNotes);
    }

    private static string RenderSection(string header, string content)
    {
        return "  <ul style='list-style-type: none;" +
               "             margin: 0;" +
               "             padding: 0;'>" +
               $"    <h2>{header}</h2>" +
               "    <div style='margin-left: 20px;'>" +
               $"      {content}" +
               "    </div>" +
               "  </ul>";
    }

    private static string RenderListItem(string label, string value)
    {
        var formattedLabel = $"<span style='font-weight: bold;'>{label}:</span>";
        var formattedValue = string.IsNullOrWhiteSpace(value)
            ? $"<span style='font-size: x-small;'>{NotSpecified}</span>"
            : value;

        return $"<li>{formattedLabel} {formattedValue}</li>";
    }
}
