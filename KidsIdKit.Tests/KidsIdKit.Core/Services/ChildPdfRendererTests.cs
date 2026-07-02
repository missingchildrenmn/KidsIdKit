using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class ChildPdfRendererTests
{
    private readonly ChildPdfRenderer _renderer = new();

    private static Child CreateChild(params SocialMediaAccount[] socialMediaAccounts)
    {
        var child = new Child();
        child.ChildDetails.GivenName = "Pat";
        child.ChildDetails.FamilyName = "Sample";
        child.SocialMediaAccounts.AddRange(socialMediaAccounts);
        return child;
    }

    private static void AssertIsPdf(byte[] result)
    {
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Expected a non-empty PDF document.");
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(result, 0, 5));
    }

    [Fact]
    public void RenderChildToPdf_WhenChildIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _renderer.RenderChildToPdf(null!));
    }

    [Fact]
    public void RenderChildToPdf_WithNoSocialMediaAccounts_ProducesPdf()
    {
        var child = CreateChild();

        var result = _renderer.RenderChildToPdf(child);

        AssertIsPdf(result);
    }

    [Fact]
    public void RenderChildToPdf_WithBrandedPlatforms_RendersBadgesWithoutError()
    {
        // Covers each branch of the brand badge rendering: a solid brand color
        // (Facebook), the CSS gradient approximation (Instagram), a dark glyph on
        // a light tile (Snapchat) and an outline glyph promoted from CSS classes
        // (Threads).
        var child = CreateChild(
            new SocialMediaAccount { Platform = "Facebook", UserName = "pat.fb", Password = "pw1" },
            new SocialMediaAccount { Platform = "Instagram", UserName = "pat.ig", Password = "pw2" },
            new SocialMediaAccount { Platform = "Snapchat", UserName = "pat.snap", Password = "pw3" },
            new SocialMediaAccount { Platform = "Threads", UserName = "pat.th", Password = "pw4" });

        var result = _renderer.RenderChildToPdf(child);

        AssertIsPdf(result);
    }

    [Fact]
    public void RenderChildToPdf_WithFreeTextPlatform_ProducesPdf()
    {
        // Unrecognized platforms have no brand icon and must still render.
        var child = CreateChild(
            new SocialMediaAccount { Platform = "MySpace", UserName = "pat.ms", Password = "pw" });

        var result = _renderer.RenderChildToPdf(child);

        AssertIsPdf(result);
    }

    [Fact]
    public void RenderChildToPdf_WithBrandedPlatform_LinksIconAndNameToProfile()
    {
        // The badge, the platform name, and the "Launch" button should all be
        // hyperlinks to the child's profile, built from the platform name and
        // username.
        var child = CreateChild(
            new SocialMediaAccount { Platform = "Facebook", UserName = "pat.fb", Password = "pw" });

        var result = _renderer.RenderChildToPdf(child);

        var uris = GetLinkUris(result);
        Assert.Contains("https://www.facebook.com/pat.fb", uris);
        Assert.Equal(3, uris.FindAll(u => u == "https://www.facebook.com/pat.fb").Count);
    }

    [Fact]
    public void RenderChildToPdf_WithFreeTextPlatform_HasNoProfileLink()
    {
        // A free-text platform has no badge, so there is nothing to link.
        var child = CreateChild(
            new SocialMediaAccount { Platform = "MySpace", UserName = "pat.ms", Password = "pw" });

        var result = _renderer.RenderChildToPdf(child);

        Assert.Empty(GetLinkUris(result));
    }

    private static List<string> GetLinkUris(byte[] pdfBytes)
    {
        var uris = new List<string>();
        using var reader = new PdfReader(new MemoryStream(pdfBytes));
        using var pdf = new PdfDocument(reader);
        for (var pageNumber = 1; pageNumber <= pdf.GetNumberOfPages(); pageNumber++)
        {
            foreach (var annotation in pdf.GetPage(pageNumber).GetAnnotations())
            {
                if (annotation is PdfLinkAnnotation link &&
                    link.GetAction() is { } action &&
                    action.GetAsString(PdfName.URI) is { } uri)
                {
                    uris.Add(uri.GetValue());
                }
            }
        }

        return uris;
    }
}
