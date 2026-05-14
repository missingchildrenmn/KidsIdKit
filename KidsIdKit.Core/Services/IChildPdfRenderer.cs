namespace KidsIdKit.Core.Services;

/// <summary>
/// Renders child data to PDF format for export.
/// </summary>
public interface IChildPdfRenderer
{
    /// <summary>
    /// Renders complete child information to a PDF document.
    /// </summary>
    /// <param name="child">The child data to render.</param>
    /// <returns>A byte array containing the generated PDF document.</returns>
    byte[] RenderChildToPdf(Data.Child child);
}
