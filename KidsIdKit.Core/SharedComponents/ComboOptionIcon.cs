namespace KidsIdKit.Core.SharedComponents;

/// <summary>
/// Describes how to render a brand-styled icon for a combobox option so each
/// option can look like its real-world app icon: a logo glyph drawn on a
/// brand-colored tile (for example Instagram's gradient or Facebook blue).
/// </summary>
/// <param name="Glyph">Ionicons glyph name, e.g. "logo-facebook".</param>
/// <param name="Background">
/// CSS background value for the tile: a solid brand color (e.g. "#1877F2") or a
/// gradient (e.g. "linear-gradient(...)").
/// </param>
/// <param name="Foreground">CSS color for the glyph. Defaults to white.</param>
public record ComboOptionIcon(string Glyph, string Background, string Foreground = "#ffffff");
