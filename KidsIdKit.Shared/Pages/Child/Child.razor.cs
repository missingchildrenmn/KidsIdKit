using Microsoft.AspNetCore.Components;
using System.IO;

namespace KidsIdKit.Shared.Pages.Child;
public partial class Child
{
    [Parameter]
    public int Id { get; set; }
    Data.Child? CurrentChild;
    private string? TemplateString { get; set; }
    private string? noneSpecified = "[none specified]";
    private string? notSpecified = "[not specified]";

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
            StoreAllInfoInHtmlString();
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

    private void StoreAllInfoInHtmlString()
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
                             $"    <h1>{CurrentChild.ChildDetails.FullName}</h1>" +
                              "  </div>" +
                             $"  <h6>Printed on {dayOfWeek} {day} at {time}</h6>" +
                             $"  {ChildDetails()}" +
                             $"  {PhysicalDetails()}" +
                              string.Concat(Enumerable.Repeat("<br />", 2)) +   // Force to the next page
                             $"  {DistinguishingFeatures()}" +
                             $"  {FamilyMembers()}" +
                             $"  {Friends()}" +
                             $"  {CareProviders()}" +
                             $"  {MedicalNotes()}" +
                              "</div>";
        }

        string ChildDetails()
        {
            var ageAndBirthday = $"{CurrentChild.ChildDetails.AgeFormatted} (born {CurrentChild.ChildDetails.Birthday.ToString("d")})";
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
            var physicalDetails =
                   $"    {li("Measurement date", CurrentChild.PhysicalDetails.MeasurementDate.ToString("d"))}" +
                   $"    {li("Height", CurrentChild.PhysicalDetails.Height)}" +
                   $"    {li("Hair color", CurrentChild.PhysicalDetails.HairColor)}" +
                   $"    {li("Hair style", CurrentChild.PhysicalDetails.HairStyle)}" +
                   $"    {li("Eye color", CurrentChild.PhysicalDetails.EyeColor)} " +
                   $"    {li("Wears contacts", CurrentChild.PhysicalDetails.EyeContacts.ToString())}" +
                   $"    {li("Eye glasses", CurrentChild.PhysicalDetails.EyeGlasses.ToString())}" +
                   $"    {li("Skin tone", CurrentChild.PhysicalDetails.SkinTone)}" +
                   $"    {li("Racial / ethnic identity", CurrentChild.PhysicalDetails.RacialEthnicIdentity)}" +
                   $"    {li("Sex", CurrentChild.PhysicalDetails.Sex)}" +
                   $"    {li("Gender identity", CurrentChild.PhysicalDetails.GenderIdentity)}";
            return $"{header_div("Physical Details", physicalDetails)}";
        }

        string DistinguishingFeatures()
        {
            var distinguishingFeatures = CurrentChild.DistinguishingFeatures;
            string distinguishingFeaturesData;

            if (distinguishingFeatures.Count == 0)
            {
                distinguishingFeaturesData = noneSpecified;
            }
            else
            {
                distinguishingFeaturesData = string.Empty;
                foreach (var distinguishingFeature in CurrentChild.DistinguishingFeatures)
                {
                    var description = distinguishingFeature.Description ?? notSpecified;
                    var photoHtml = distinguishingFeature.Photo?.ImageSource == null
                        ? notSpecified
                        : $"<img src='{distinguishingFeature.Photo.ImageSource}' title='Photo of Distinguishing feature' alt='Photo of Distinguishing feature' style='max-height: 150px;' />";
                    distinguishingFeaturesData +=
                       "<tr>" +
                      $"  <td>{description}</td>" +
                      $"  <td>{photoHtml}</td>" +
                       "</tr>";
                }

                distinguishingFeaturesData =
                        " <table style='width: 100%'>" +
                        "  <tr>" +
                        "    <th style='width: 30%;'>Description</th>" +
                        "    <th>Photo</th>" +
                        "  </tr>" +
                       $"    {distinguishingFeaturesData}" +
                        "</table>";
            }

            return $"{header_div("Distinguishing Features", distinguishingFeaturesData)}";
        }

        string FamilyMembers()
        {
            var familyMembers = CurrentChild.FamilyMembers;
            string familyMembersData;

            if (familyMembers.Count == 0)
            {
                familyMembersData = noneSpecified;
            }
            else
            {
                familyMembersData = string.Empty;
                foreach (var familyMember in familyMembers)
                {
                    var givenName = familyMember.GivenName ?? notSpecified;
                    var nickname = familyMember.NickName ?? notSpecified;
                    var familyName = familyMember.FamilyName ?? notSpecified;
                    var relation = familyMember.Relation ?? notSpecified;
                    var address = familyMember.Address ?? notSpecified;
                    var phoneNumber = familyMember.PhoneNumber ?? notSpecified;
                    familyMembersData +=
                       "<tr>" +
                      $"  <td>{givenName}</td>" +
                      $"  <td>{nickname}</td>" +
                      $"  <td>{familyName}</td>" +
                      $"  <td>{relation}</td>" +
                      $"  <td>{address}</td>" +
                      $"  <td>{phoneNumber}</td>" +
                       "</tr>";
                }

                familyMembersData =
                        " <table style='width: 100%'>" +
                        "  <tr>" +
                        "    <th style='width: 30%;'>Given Name</th>" +
                        "    <th>Nickname</th>" +
                        "    <th>Family Name</th>" +
                        "    <th>Relation</th>" +
                        "    <th>Address</th>" +
                        "    <th>Phone Number</th>" +
                        "  </tr>" +
                       $"    {familyMembersData}" +    // Insert the data into the table
                        "</table>";
            }

            return $"{header_div("Family Members", familyMembersData)}";
        }

        string Friends()
        {
            string friendsData;

            var friends = CurrentChild.Friends;
            if (friends.Count == 0)
            {
                friendsData = noneSpecified;
            }
            else
            {
                friendsData = string.Empty;
                foreach (var friend in friends)
                {
                    var givenName = friend.GivenName ?? notSpecified;
                    var nickname = friend.NickName ?? notSpecified;
                    var familyName = friend.FamilyName ?? notSpecified;
                    var address = friend.Address ?? notSpecified;
                    var phoneNumber = friend.PhoneNumber ?? notSpecified;
                    friendsData +=
                       "<tr>" +
                      $"  <td>{givenName}</td>" +
                      $"  <td>{nickname}</td>" +
                      $"  <td>{familyName}</td>" +
                      $"  <td>{address}</td>" +
                      $"  <td>{phoneNumber}</td>" +
                       "</tr>";
                }

                friendsData =
                        " <table style='width: 100%'>" +
                        "  <tr>" +
                        "    <th style='width: 30%;'>Given Name</th>" +
                        "    <th>Nickname</th>" +
                        "    <th>Family Name</th>" +
                        "    <th>Address</th>" +
                        "    <th>Phone Number</th>" +
                        "  </tr>" +
                       $"    {friendsData}" +
                        "</table>";
            }

            return $"{header_div("Friends", friendsData)}";
        }

        string CareProviders()
        {
            string careProvidersData;

            var careProviders = CurrentChild.ProfessionalCareProviders;
            if (careProviders.Count == 0)
            {
                careProvidersData = noneSpecified;
            }
            else
            {
                careProvidersData = string.Empty;
                foreach (var careProvider in careProviders)
                {
                    var clinicName = careProvider.ClinicName ?? notSpecified;
                    var givenName = careProvider.GivenName ?? notSpecified;
                    var familyName = careProvider.FamilyName ?? notSpecified;
                    var role = careProvider.CareRoleDescription ?? notSpecified;
                    var phoneNumber = careProvider.PhoneNumber ?? notSpecified;
                    var address = careProvider.Address ?? notSpecified;
                    careProvidersData +=
                       "<tr>" +
                      $"  <td>{clinicName}</td>" +
                      $"  <td>{givenName}</td>" +
                      $"  <td>{familyName}</td>" +
                      $"  <td>{role}</td>" +
                      $"  <td>{phoneNumber}</td>" +
                      $"  <td>{address}</td>" +
                       "</tr>";
                }

                careProvidersData =
                        " <table style='width: 100%'>" +
                        "  <tr>" +
                        "    <th style='width: 30%;'>Clinic Name</th>" +
                        "    <th>Given Name</th>" +
                        "    <th>Family Name</th>" +
                        "    <th>Role</th>" +
                        "    <th>Phone Number</th>" +
                        "    <th>Address</th>" +
                        "  </tr>" +
                       $"    {careProvidersData}" +
                        "</table>";
            }

            return $"{header_div("Care Providers", careProvidersData)}";
        }

        string MedicalNotes()
        {
            var medicalNotes =
                   $"    {li("MedicAlertInfo", CurrentChild.MedicalNotes.MedicAlertInfo)}" +
                   $"    {li("Allergies", CurrentChild.MedicalNotes.Allergies)}" +
                   $"    {li("RegularMedications", CurrentChild.MedicalNotes.RegularMedications)}" +
                   $"    {li("Psychiatric Medications", CurrentChild.MedicalNotes.PsychMedications)}" +
                   $"    {li("Notes", CurrentChild.MedicalNotes.Notes)} " +
                   $"    {li("Inhaler", CurrentChild.MedicalNotes.Inhaler.ToString())}" +
                   $"    {li("Diabetic", CurrentChild.MedicalNotes.Diabetic.ToString())}";

            return $"{header_div("Medical Notes", medicalNotes)}";
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
                                                        ? $"<span style='font-size: x-small;'>{notSpecified}</span>"
                                                        : value) +
                   "</li>";
        }
    }

    private async Task SendAllInfo()
    {
        var filename = $"{CurrentChild!.ChildDetails.FullName.Replace(' ', '-')}.html";

        if (await FileSaverService.SaveFileAsync(filename, TemplateString)) {
            await FileSharerService.ShareFileAsync(filename);
        }
    }
}
