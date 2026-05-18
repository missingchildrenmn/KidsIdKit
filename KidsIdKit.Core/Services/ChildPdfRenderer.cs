using System;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using KidsIdKit.Core.Data;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Renders child data to a PDF document for export.
/// </summary>
public class ChildPdfRenderer : IChildPdfRenderer
{
    private const string NoneSpecified = "[none specified]";
    private const string NotSpecified = "[not specified]";
    private const float PhotoMaxHeight = 330f;

    /// <inheritdoc />
    public byte[] RenderChildToPdf(Child child)
    {
        ArgumentNullException.ThrowIfNull(child);

        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        using var stream = new MemoryStream();
        using (var writer = new PdfWriter(stream))
        using (var pdf = new PdfDocument(writer))
        using (var document = new Document(pdf))
        {
            var today = DateTime.Now;
            var dayOfWeek = today.DayOfWeek;
            var day = today.ToString("M/d/yyyy");
            var time = today.ToString("HH:mm:ss");

            document.Add(new Paragraph("Kids ID Kit information for")
                .SetFontSize(10f));
            document.Add(new Paragraph(child.ChildDetails.FullName ?? string.Empty)
                .SetFont(boldFont)
                .SetFontSize(20f));
            document.Add(new Paragraph($"Printed on {dayOfWeek} {day} at {time}")
                .SetFontSize(10f)
                .SetMarginBottom(8f));

            AddChildDetails(document, child, boldFont);
            AddPhysicalDetails(document, child, boldFont);
            AddDistinguishingFeatures(document, child, boldFont);
            AddFamilyMembers(document, child, boldFont);
            AddFriends(document, child, boldFont);
            AddCareProviders(document, child, boldFont);
            AddMedicalNotes(document, child, boldFont);
        }

        return stream.ToArray();
    }

    private static void AddChildDetails(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Child Details", boldFont);

        AddLabeledValue(document, "Given name", child.ChildDetails.GivenName, boldFont);
        AddLabeledValue(document, "Middle name", child.ChildDetails.MiddleName, boldFont);
        AddLabeledValue(document, "Family name", child.ChildDetails.FamilyName, boldFont);
        AddLabeledValue(document, "Nickname", child.ChildDetails.NickName, boldFont);
        AddLabeledValue(document, "Additional name", child.ChildDetails.AdditionalName, boldFont);
        AddLabeledValue(document, "Age", $"{child.ChildDetails.AgeFormatted} (born {child.ChildDetails.Birthday:d})", boldFont);
        AddLabeledValue(document, "Phone number", child.ChildDetails.PhoneNumber, boldFont);

        var photo = TryCreateImage(child.ChildDetails.Photo?.ImageSource);
        if (photo != null)
        {
            document.Add(new Paragraph().Add(new Text("Photo:").SetFont(boldFont)));
            document.Add(photo.SetMarginLeft(20f));
        }
        else
        {
            AddLabeledValue(document, "Photo", null, boldFont);
        }
    }

    private static void AddPhysicalDetails(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Physical Details", boldFont);

        AddLabeledValue(document, "Height", child.PhysicalDetails.Height, boldFont);
        AddLabeledValue(document, "Weight", child.PhysicalDetails.Weight, boldFont);
        AddLabeledValue(document, "Measurement date", child.PhysicalDetails.MeasurementDate.ToString("d"), boldFont);
        AddLabeledValue(document, "Hair color", child.PhysicalDetails.HairColor, boldFont);
        AddLabeledValue(document, "Hair style", child.PhysicalDetails.HairStyle, boldFont);
        AddLabeledValue(document, "Eye color", child.PhysicalDetails.EyeColor, boldFont);
        AddLabeledValue(document, "Wears contacts", BoolToString(child.PhysicalDetails.EyeContacts), boldFont);
        AddLabeledValue(document, "Eye glasses", BoolToString(child.PhysicalDetails.EyeGlasses), boldFont);
        AddLabeledValue(document, "Skin tone", child.PhysicalDetails.SkinTone, boldFont);
        AddLabeledValue(document, "Racial / ethnic identity", child.PhysicalDetails.RacialEthnicIdentity, boldFont);
        AddLabeledValue(document, "Sex", child.PhysicalDetails.Sex, boldFont);
        AddLabeledValue(document, "Gender identity", child.PhysicalDetails.GenderIdentity, boldFont);
    }

    private static string BoolToString(bool? value)
    {
        return value.HasValue ? (value.Value ? "Yes" : "No") : NotSpecified;
    }

    private static void AddDistinguishingFeatures(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Distinguishing Features", boldFont);

        if (child.DistinguishingFeatures.Count == 0)
        {
            document.Add(new Paragraph(NoneSpecified).SetMarginLeft(20f));
            return;
        }

        var table = CreateTable(new float[] { 3f, 7f }, boldFont, "Description", "Photo");
        foreach (var feature in child.DistinguishingFeatures)
        {
            table.AddCell(CreateTextCell(feature.Description ?? NotSpecified));
            var photo = TryCreateImage(feature.Photo?.ImageSource);
            table.AddCell(photo != null ? new Cell().Add(photo) : CreateTextCell(NotSpecified));
        }
        document.Add(table);
    }

    private static void AddFamilyMembers(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Family Members", boldFont);

        if (child.FamilyMembers.Count == 0)
        {
            document.Add(new Paragraph(NoneSpecified).SetMarginLeft(20f));
            return;
        }

        var table = CreateTable(
            new float[] { 2f, 1.5f, 2f, 2f, 3f, 2f },
            boldFont,
            "Given Name", "Nickname", "Family Name", "Relation", "Address", "Phone Number");
        foreach (var member in child.FamilyMembers)
        {
            table.AddCell(CreateTextCell(member.GivenName ?? NotSpecified));
            table.AddCell(CreateTextCell(member.NickName ?? NotSpecified));
            table.AddCell(CreateTextCell(member.FamilyName ?? NotSpecified));
            table.AddCell(CreateTextCell(member.Relation ?? NotSpecified));
            table.AddCell(CreateTextCell(member.Address ?? NotSpecified));
            table.AddCell(CreateTextCell(member.PhoneNumber ?? NotSpecified));
        }
        document.Add(table);
    }

    private static void AddFriends(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Friends", boldFont);

        if (child.Friends.Count == 0)
        {
            document.Add(new Paragraph(NoneSpecified).SetMarginLeft(20f));
            return;
        }

        var table = CreateTable(
            new float[] { 2f, 1.5f, 2f, 3f, 2f },
            boldFont,
            "Given Name", "Nickname", "Family Name", "Address", "Phone Number");
        foreach (var friend in child.Friends)
        {
            table.AddCell(CreateTextCell(friend.GivenName ?? NotSpecified));
            table.AddCell(CreateTextCell(friend.NickName ?? NotSpecified));
            table.AddCell(CreateTextCell(friend.FamilyName ?? NotSpecified));
            table.AddCell(CreateTextCell(friend.Address ?? NotSpecified));
            table.AddCell(CreateTextCell(friend.PhoneNumber ?? NotSpecified));
        }
        document.Add(table);
    }

    private static void AddCareProviders(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Care Providers", boldFont);

        if (child.ProfessionalCareProviders.Count == 0)
        {
            document.Add(new Paragraph(NoneSpecified).SetMarginLeft(20f));
            return;
        }

        var table = CreateTable(
            new float[] { 3f, 2f, 2f, 2f, 2f, 3f },
            boldFont,
            "Clinic Name", "Given Name", "Family Name", "Role", "Phone Number", "Address");
        foreach (var provider in child.ProfessionalCareProviders)
        {
            table.AddCell(CreateTextCell(provider.ClinicName ?? NotSpecified));
            table.AddCell(CreateTextCell(provider.GivenName ?? NotSpecified));
            table.AddCell(CreateTextCell(provider.FamilyName ?? NotSpecified));
            table.AddCell(CreateTextCell(provider.CareRoleDescription ?? NotSpecified));
            table.AddCell(CreateTextCell(provider.PhoneNumber ?? NotSpecified));
            table.AddCell(CreateTextCell(provider.Address ?? NotSpecified));
        }
        document.Add(table);
    }

    private static void AddMedicalNotes(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Medical Notes", boldFont);

        AddLabeledValue(document, "Medical Alert Info.", child.MedicalNotes.MedicAlertInfo, boldFont);
        AddLabeledValue(document, "Allergies", child.MedicalNotes.Allergies, boldFont);
        AddLabeledValue(document, "Regular Medications", child.MedicalNotes.RegularMedications, boldFont);
        AddLabeledValue(document, "Psychiatric Medications", child.MedicalNotes.PsychMedications, boldFont);
        AddLabeledValue(document, "Notes", child.MedicalNotes.Notes, boldFont);
        AddLabeledValue(document, "Inhaler", BoolToString(child.MedicalNotes.Inhaler), boldFont);
        AddLabeledValue(document, "Diabetic", BoolToString(child.MedicalNotes.Diabetic), boldFont);
    }

    private static void AddSectionHeader(Document document, string header, PdfFont boldFont)
    {
        document.Add(new Paragraph(header)
            .SetFont(boldFont)
            .SetFontSize(14f)
            .SetMarginTop(10f)
            .SetMarginBottom(4f));
    }

    private static void AddLabeledValue(Document document, string label, string? value, PdfFont boldFont)
    {
        var paragraph = new Paragraph()
            .SetMarginLeft(20f)
            .SetMarginBottom(2f);

        paragraph.Add(new Text($"{label}: ").SetFont(boldFont));

        if (string.IsNullOrWhiteSpace(value))
        {
            paragraph.Add(new Text(NotSpecified).SetFontSize(8f));
        }
        else
        {
            paragraph.Add(new Text(value));
        }

        document.Add(paragraph);
    }

    private static Table CreateTable(float[] columnWidths, PdfFont boldFont, params string[] headers)
    {
        var table = new Table(UnitValue.CreatePercentArray(columnWidths))
            .UseAllAvailableWidth()
            .SetMarginLeft(0f)
            .SetMarginBottom(8f);

        foreach (var header in headers)
        {
            table.AddHeaderCell(new Cell()
                .Add(new Paragraph(header).SetFont(boldFont))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetBorder(new SolidBorder(0.5f)));
        }

        return table;
    }

    private static Cell CreateTextCell(string text)
    {
        return new Cell()
            .Add(new Paragraph(text))
            .SetBorder(new SolidBorder(0.5f));
    }

    private static Image? TryCreateImage(string? imageSource)
    {
        if (string.IsNullOrWhiteSpace(imageSource))
        {
            return null;
        }

        try
        {
            var bytes = DecodeImageSource(imageSource);
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var imageData = ImageDataFactory.Create(bytes);
            var image = new Image(imageData);

            // Cameras often store the pixel data in the sensor's native orientation and
            // record the intended display orientation in an EXIF tag. iText does not
            // apply EXIF orientation automatically, so do it here.
            var orientation = ReadJpegExifOrientation(bytes);
            ApplyExifOrientation(image, orientation);

            // Use the post-rotation dimensions for scaling.
            var displayHeight = orientation is 5 or 6 or 7 or 8
                ? image.GetImageWidth()
                : image.GetImageHeight();
            var displayWidth = orientation is 5 or 6 or 7 or 8
                ? image.GetImageHeight()
                : image.GetImageWidth();

            // Constrain to a reasonable display size while preserving aspect ratio.
            if (displayHeight > PhotoMaxHeight)
            {
                var scale = PhotoMaxHeight / displayHeight;
                image.ScaleAbsolute(displayWidth * scale, PhotoMaxHeight);
            }

            return image;
        }
        catch
        {
            return null;
        }
    }

    private static byte[]? DecodeImageSource(string imageSource)
    {
        // Supports "data:[<mime>];base64,<data>" data URIs as produced by PhotoPicker.
        const string base64Marker = ";base64,";
        var markerIndex = imageSource.IndexOf(base64Marker, StringComparison.OrdinalIgnoreCase);
        var base64 = markerIndex >= 0
            ? imageSource[(markerIndex + base64Marker.Length)..]
            : imageSource;

        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static void ApplyExifOrientation(Image image, int orientation)
    {
        // EXIF orientation values (per the EXIF 2.3 spec). iText's SetRotationAngle
        // rotates counter-clockwise, expressed in radians.
        switch (orientation)
        {
            case 3: // 180°
                image.SetRotationAngle(Math.PI);
                break;
            case 6: // 90° clockwise (image stored rotated 90° CCW)
                image.SetRotationAngle(-Math.PI / 2);
                break;
            case 8: // 90° counter-clockwise
                image.SetRotationAngle(Math.PI / 2);
                break;
            // Cases 2/4/5/7 involve a mirror flip that iText cannot express with a
            // single rotation; they are uncommon for camera output. Fall through and
            // leave the image as-is rather than mis-rotating it.
            default:
                break;
        }
    }

    /// <summary>
    /// Returns the EXIF orientation tag (1–8) from a JPEG byte stream, or 1 if
    /// no orientation tag is present or the data is not a recognizable JPEG.
    /// </summary>
    private static int ReadJpegExifOrientation(byte[] bytes)
    {
        const int defaultOrientation = 1;

        if (bytes.Length < 4 || bytes[0] != 0xFF || bytes[1] != 0xD8)
        {
            return defaultOrientation;
        }

        var i = 2;
        while (i + 4 <= bytes.Length && bytes[i] == 0xFF)
        {
            var marker = bytes[i + 1];

            // Skip fill bytes between segments.
            if (marker == 0xFF)
            {
                i++;
                continue;
            }

            // SOI/EOI/RSTn have no payload.
            if (marker == 0xD8 || marker == 0xD9 || (marker >= 0xD0 && marker <= 0xD7))
            {
                i += 2;
                continue;
            }

            if (i + 4 > bytes.Length)
            {
                return defaultOrientation;
            }

            var segmentLength = (bytes[i + 2] << 8) | bytes[i + 3];
            if (segmentLength < 2 || i + 2 + segmentLength > bytes.Length)
            {
                return defaultOrientation;
            }

            // APP1 segment containing EXIF.
            if (marker == 0xE1 && segmentLength >= 8 + 2 &&
                bytes[i + 4] == 0x45 && bytes[i + 5] == 0x78 &&
                bytes[i + 6] == 0x69 && bytes[i + 7] == 0x66 &&
                bytes[i + 8] == 0x00 && bytes[i + 9] == 0x00)
            {
                var tiffStart = i + 10;
                return ReadOrientationFromTiff(bytes, tiffStart, i + 2 + segmentLength) ?? defaultOrientation;
            }

            i += 2 + segmentLength;
        }

        return defaultOrientation;
    }

    private static int? ReadOrientationFromTiff(byte[] bytes, int tiffStart, int tiffEnd)
    {
        if (tiffStart + 8 > tiffEnd)
        {
            return null;
        }

        bool littleEndian;
        if (bytes[tiffStart] == 0x49 && bytes[tiffStart + 1] == 0x49)
        {
            littleEndian = true;
        }
        else if (bytes[tiffStart] == 0x4D && bytes[tiffStart + 1] == 0x4D)
        {
            littleEndian = false;
        }
        else
        {
            return null;
        }

        ushort magic = ReadUInt16(bytes, tiffStart + 2, littleEndian);
        if (magic != 0x002A)
        {
            return null;
        }

        uint ifdOffset = ReadUInt32(bytes, tiffStart + 4, littleEndian);
        var ifd0 = tiffStart + (int)ifdOffset;
        if (ifd0 < tiffStart || ifd0 + 2 > tiffEnd)
        {
            return null;
        }

        ushort entryCount = ReadUInt16(bytes, ifd0, littleEndian);
        var entriesStart = ifd0 + 2;
        if (entriesStart + entryCount * 12 > tiffEnd)
        {
            return null;
        }

        for (var e = 0; e < entryCount; e++)
        {
            var entry = entriesStart + e * 12;
            ushort tag = ReadUInt16(bytes, entry, littleEndian);
            if (tag == 0x0112) // Orientation
            {
                ushort value = ReadUInt16(bytes, entry + 8, littleEndian);
                if (value >= 1 && value <= 8)
                {
                    return value;
                }
                return null;
            }
        }

        return null;
    }

    private static ushort ReadUInt16(byte[] bytes, int offset, bool littleEndian) =>
        littleEndian
            ? (ushort)(bytes[offset] | (bytes[offset + 1] << 8))
            : (ushort)((bytes[offset] << 8) | bytes[offset + 1]);

    private static uint ReadUInt32(byte[] bytes, int offset, bool littleEndian) =>
        littleEndian
            ? (uint)(bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24))
            : (uint)((bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3]);
}
