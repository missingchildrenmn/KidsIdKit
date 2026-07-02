using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Svg.Converter;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Pages.SocialMediaAccounts;
using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Renders child data to a PDF document for export.
/// </summary>
public class ChildPdfRenderer : IChildPdfRenderer
{
    private const string NoneSpecified = "[none specified]";
    private const string NotSpecified = "[not specified]";
    private const float PhotoMaxHeight = 330f;

    // Side length (in points) of the brand badge drawn next to a platform name,
    // matching the icon-tile look used on the Social Media Accounts page.
    private const float PlatformBadgeSize = 14f;

    // Background fill for the small "Launch" button rendered next to a linked
    // platform name, approximating the app's secondary action button.
    private static readonly Color LaunchButtonColor = new DeviceRgb(56, 128, 255);

    private static readonly Assembly CoreAssembly = typeof(ChildPdfRenderer).Assembly;

    // Maps an Ionicons glyph name (e.g. "logo-facebook") to the embedded SVG
    // resource that holds its artwork, resolved once from the assembly manifest.
    private static readonly Dictionary<string, string> GlyphResourceNames = BuildGlyphResourceNames();

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
            AddSocialMediaAccounts(document, child, boldFont);
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

    private static void AddSocialMediaAccounts(Document document, Child child, PdfFont boldFont)
    {
        AddSectionHeader(document, "Social Media Accounts", boldFont);

        if (child.SocialMediaAccounts.Count == 0)
        {
            document.Add(new Paragraph(NoneSpecified).SetMarginLeft(20f));
            return;
        }

        var pdf = document.GetPdfDocument();
        var table = CreateTable(
            new float[] { 2f, 3f, 3f },
            boldFont,
            "Platform", "User Name", "Password");
        foreach (var account in child.SocialMediaAccounts)
        {
            table.AddCell(CreatePlatformCell(pdf, account.Platform, account.UserName));
            table.AddCell(CreateTextCell(account.UserName ?? NotSpecified));
            table.AddCell(CreateTextCell(account.Password ?? NotSpecified));
        }
        document.Add(table);
    }

    // Builds the Platform cell so it mirrors the app: a brand-colored rounded
    // tile bearing the platform's logo (linked to the child's profile on that
    // platform), followed by the platform name. Free-text or unrecognized
    // platforms have no icon and render as plain text.
    private static Cell CreatePlatformCell(PdfDocument pdf, string? platform, string? userName)
    {
        var name = platform ?? NotSpecified;
        var icon = SocialMediaPlatformIcons.Get(platform);
        var badge = icon != null ? TryCreatePlatformBadge(pdf, icon, PlatformBadgeSize) : null;

        // Link the brand badge and the platform name to the child's profile on
        // that platform, matching the Social Media Accounts page. Accounts without
        // a usable username get no link (the badge/name still render).
        var profileUrl = SocialMediaPlatformUrls.GetProfileUrl(platform, userName);
        if (badge != null && profileUrl != null)
        {
            badge.SetAction(PdfAction.CreateURI(profileUrl));
        }

        ILeafElement nameElement = profileUrl != null
            ? new Link(name, PdfAction.CreateURI(profileUrl))
            : new Text(name);

        // Mirror the app: when the profile is reachable, offer a small "Launch"
        // button (icon + label) next to the name that opens the same profile.
        var launchButton = profileUrl != null ? TryCreateLaunchButton(pdf, profileUrl) : null;

        if (badge == null)
        {
            return new Cell()
                .Add(BuildNameWithLaunch(nameElement, launchButton))
                .SetBorder(new SolidBorder(0.5f));
        }

        // Lay the badge and name out as two borderless cells so the row grows to
        // fit the badge and both stay vertically centered. Adding the badge inline
        // to a paragraph clips its top against the (shorter) text line box.
        var layout = new Table(UnitValue.CreatePercentArray(new float[] { 1f, 5f }))
            .SetWidth(UnitValue.CreatePercentValue(100f))
            .SetBorder(Border.NO_BORDER);

        layout.AddCell(new Cell()
            .Add(badge)
            .SetBorder(Border.NO_BORDER)
            .SetPadding(0f)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        layout.AddCell(new Cell()
            .Add(BuildNameWithLaunch(nameElement, launchButton))
            .SetBorder(Border.NO_BORDER)
            .SetPadding(0f)
            .SetPaddingLeft(5f)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        return new Cell()
            .Add(layout)
            .SetBorder(new SolidBorder(0.5f));
    }

    // Lays out the platform name with an optional launch button to its right,
    // both vertically centered so the taller button image is not clipped by the
    // text line box. Without a button the name renders as a plain paragraph.
    private static IBlockElement BuildNameWithLaunch(ILeafElement nameElement, Image? launchButton)
    {
        var nameParagraph = new Paragraph().Add(nameElement).SetMargin(0f);
        if (launchButton == null)
        {
            return nameParagraph;
        }

        var layout = new Table(2)
            .SetAutoLayout()
            .SetBorder(Border.NO_BORDER);

        layout.AddCell(new Cell()
            .Add(nameParagraph)
            .SetBorder(Border.NO_BORDER)
            .SetPadding(0f)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        layout.AddCell(new Cell()
            .Add(launchButton)
            .SetBorder(Border.NO_BORDER)
            .SetPadding(0f)
            .SetPaddingLeft(5f)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        return layout;
    }

    // Draws a small "Launch" button into a form XObject so it mirrors the app's
    // button: a blue rounded pill bearing the white open-outline icon followed
    // by a white "Launch" label. The returned image carries the profile URI
    // action so the whole button is clickable. Returns null on failure so the
    // caller can fall back to just the name link.
    private static Image? TryCreateLaunchButton(PdfDocument pdf, string profileUrl)
    {
        try
        {
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            const float fontSize = 8f;
            const float height = 13f;
            const float iconSize = 8f;
            const float paddingX = 5f;
            const float gap = 3f;
            const string label = "Launch";

            var textWidth = font.GetWidth(label, fontSize);
            var width = paddingX + iconSize + gap + textWidth + paddingX;

            var button = new PdfFormXObject(new Rectangle(0f, 0f, width, height));
            var canvas = new PdfCanvas(button, pdf);

            var radius = height * 0.3f;
            canvas.SetFillColor(LaunchButtonColor);
            canvas.RoundRectangle(0f, 0f, width, height, radius);
            canvas.Fill();

            // The white open-outline glyph on the left of the label.
            var glyph = LoadGlyphXObject(pdf, "open-outline", "#ffffff");
            if (glyph != null)
            {
                var iconY = (height - iconSize) / 2f;
                canvas.AddXObjectFittedIntoRectangle(glyph, new Rectangle(paddingX, iconY, iconSize, iconSize));
            }

            canvas.BeginText()
                .SetFontAndSize(font, fontSize)
                .SetFillColor(ColorConstants.WHITE)
                .MoveText(paddingX + iconSize + gap, (height - fontSize) / 2f + 1.2f)
                .ShowText(label)
                .EndText();

            var image = new Image(button);
            image.SetAction(PdfAction.CreateURI(profileUrl));
            return image;
        }
        catch
        {
            return null;
        }
    }

    // Draws the brand badge into a form XObject: a rounded tile filled with the
    // platform's brand color (or a gradient approximation) topped by its logo
    // glyph, recolored to the brand foreground. Returns null if drawing fails so
    // the caller can fall back to a plain text platform name.
    private static Image? TryCreatePlatformBadge(PdfDocument pdf, ComboOptionIcon icon, float size)
    {
        try
        {
            var badge = new PdfFormXObject(new Rectangle(0f, 0f, size, size));
            var canvas = new PdfCanvas(badge, pdf);

            var radius = size * 0.25f;
            canvas.SetFillColor(BuildBackgroundColor(icon.Background, size, pdf));
            canvas.RoundRectangle(0f, 0f, size, size, radius);
            canvas.Fill();

            var glyph = LoadGlyphXObject(pdf, icon.Glyph, icon.Foreground);
            if (glyph != null)
            {
                var glyphSize = size * 2f / 3f;
                var offset = (size - glyphSize) / 2f;
                canvas.AddXObjectFittedIntoRectangle(glyph, new Rectangle(offset, offset, glyphSize, glyphSize));
            }

            return new Image(badge);
        }
        catch
        {
            return null;
        }
    }

    // Loads an embedded Ionicons SVG by glyph name and converts it to a PDF form
    // XObject, recolored to the supplied foreground so it matches the app.
    private static PdfFormXObject? LoadGlyphXObject(PdfDocument pdf, string glyph, string foreground)
    {
        if (string.IsNullOrEmpty(glyph) || !GlyphResourceNames.TryGetValue(glyph, out var resourceName))
        {
            return null;
        }

        string svg;
        using (var stream = CoreAssembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                return null;
            }

            using var reader = new StreamReader(stream);
            svg = reader.ReadToEnd();
        }

        return SvgConverter.ConvertToXObject(ColorizeSvg(svg, foreground), pdf);
    }

    // Ionicons rely on CSS (a stylesheet the PDF SVG converter does not load) to
    // color their glyphs. Promote that styling to explicit attributes: outline
    // glyphs get an explicit none-fill plus a brand-colored stroke, and every
    // glyph gets an explicit size and a brand-colored fill.
    private static string ColorizeSvg(string svg, string foreground)
    {
        svg = svg.Replace(
            "class=\"ionicon-fill-none ionicon-stroke-width\"",
            $"fill=\"none\" stroke=\"{foreground}\" stroke-width=\"32\"");

        svg = svg.Replace(
            "<svg ",
            $"<svg width=\"512\" height=\"512\" fill=\"{foreground}\" ");

        // Filled brand logos (e.g., logo-facebook) declare no fill on their paths
        // and rely on inheriting it from the root <svg>. iText's SVG converter
        // does not propagate that inherited fill, so the glyph renders invisibly.
        // Set the brand fill explicitly on every drawable element that does not
        // already declare its own fill (outline paths keep their fill="none").
        svg = Regex.Replace(
            svg,
            "<(path|circle|ellipse|rect|polygon|polyline|line)\\b(?![^>]*\\bfill=)",
            $"<$1 fill=\"{foreground}\"");

        return svg;
    }

    // Resolves the tile background. Solid brand colors map directly; CSS
    // gradients (e.g. Instagram's) are approximated with a diagonal linear
    // gradient over the same color stops since iText has no CSS gradient parser.
    private static Color BuildBackgroundColor(string? background, float size, PdfDocument pdf)
    {
        if (!string.IsNullOrWhiteSpace(background))
        {
            if (TryParseHexColor(background, out var solid))
            {
                return solid;
            }

            var stops = ExtractGradientStops(background);
            if (stops.Count == 1)
            {
                return stops[0].Color;
            }

            if (stops.Count >= 2)
            {
                try
                {
                    // Approximate the CSS radial-gradient (its center sits near the
                    // bottom-left, e.g. Instagram's "circle at 30% 107%") with a
                    // linear gradient running bottom-left -> top-right, and honor
                    // each stop's real offset. This keeps the light stops in the
                    // lower-left corner and the darker stops over the top/left,
                    // where the white glyph needs contrast, so it isn't washed out.
                    AbstractLinearGradientBuilder builder = new LinearGradientBuilder()
                        .SetGradientVector(0f, 0f, size, size)
                        .SetSpreadMethod(GradientSpreadMethod.PAD);
                    foreach (var stop in stops)
                    {
                        builder.AddColorStop(new GradientColorStop(
                            stop.Color.GetColorValue(), stop.Offset ?? 0d, GradientColorStop.OffsetType.RELATIVE));
                    }

                    return builder.BuildColor(new Rectangle(0f, 0f, size, size), null, pdf);
                }
                catch
                {
                    return stops[stops.Count / 2].Color;
                }
            }
        }

        return new DeviceRgb(0x60, 0x60, 0x60);
    }

    // Parses the color stops of a CSS gradient string into (color, offset) pairs.
    // Each stop may carry an explicit percentage (e.g. "#fd5949 45%"); stops with
    // no percentage are distributed evenly across any gap, mirroring CSS behavior.
    // Honoring the real offsets keeps narrow stops (like Instagram's 0%-5% cream
    // sliver) in the corner instead of letting them flood a quarter of the tile.
    private static List<GradientStop> ExtractGradientStops(string value)
    {
        var stops = new List<GradientStop>();
        foreach (Match match in Regex.Matches(
            value,
            @"(?<hex>#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{3}))\s*(?<pct>\d+(?:\.\d+)?%)?"))
        {
            if (!TryParseHexColor(match.Groups["hex"].Value, out var color))
            {
                continue;
            }

            double? offset = null;
            var pct = match.Groups["pct"];
            if (pct.Success &&
                double.TryParse(pct.Value.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                offset = Math.Clamp(parsed / 100d, 0d, 1d);
            }

            stops.Add(new GradientStop(color, offset));
        }

        FillMissingOffsets(stops);
        return stops;
    }

    // Assigns offsets to any stop that lacked an explicit percentage, spreading
    // them evenly between the nearest defined neighbors (CSS gradient semantics).
    private static void FillMissingOffsets(List<GradientStop> stops)
    {
        if (stops.Count == 0)
        {
            return;
        }

        if (stops[0].Offset is null)
        {
            stops[0] = stops[0] with { Offset = 0d };
        }

        if (stops[stops.Count - 1].Offset is null)
        {
            stops[stops.Count - 1] = stops[stops.Count - 1] with { Offset = 1d };
        }

        var i = 0;
        while (i < stops.Count)
        {
            if (stops[i].Offset is not null)
            {
                i++;
                continue;
            }

            var start = i - 1;
            var end = i;
            while (end < stops.Count && stops[end].Offset is null)
            {
                end++;
            }

            var startOffset = stops[start].Offset!.Value;
            var endOffset = stops[end].Offset!.Value;
            var gaps = end - start;
            for (var j = start + 1; j < end; j++)
            {
                var fraction = (double)(j - start) / gaps;
                stops[j] = stops[j] with { Offset = startOffset + (endOffset - startOffset) * fraction };
            }

            i = end;
        }
    }

    private readonly record struct GradientStop(DeviceRgb Color, double? Offset);

    private static bool TryParseHexColor(string value, out DeviceRgb color)
    {
        color = null!;

        var hex = value?.Trim();
        if (string.IsNullOrEmpty(hex) || hex[0] != '#')
        {
            return false;
        }

        hex = hex.Substring(1);
        if (hex.Length == 3)
        {
            hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
        }

        if (hex.Length != 6 ||
            !int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
        {
            return false;
        }

        color = new DeviceRgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
        return true;
    }

    private static Dictionary<string, string> BuildGlyphResourceNames()
    {
        const string marker = ".Assets.PlatformIcons.";
        const string extension = ".svg";

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in CoreAssembly.GetManifestResourceNames())
        {
            var markerIndex = name.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0 || !name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var glyphStart = markerIndex + marker.Length;
            var glyph = name.Substring(glyphStart, name.Length - glyphStart - extension.Length);
            map[glyph] = name;
        }

        return map;
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
