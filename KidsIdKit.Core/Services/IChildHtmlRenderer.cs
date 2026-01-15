namespace KidsIdKit.Core.Services;

/// <summary>
/// Renders child data to HTML format for display and export.
/// </summary>
public interface IChildHtmlRenderer
{
    /// <summary>
    /// Renders complete child information to an HTML string.
    /// </summary>
    /// <param name="child">The child data to render.</param>
    /// <returns>HTML string containing all child information.</returns>
    string RenderChildToHtml(Data.Child child);
}
