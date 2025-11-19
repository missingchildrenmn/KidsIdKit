namespace KidsIdKit.Core.Data;

public class Photo
{
    public string? FileName { get; set; }
    public DateTimeOffset DateSelected { get; set; } = DateTimeOffset.Now;
    public string? ImageSource { get; set; }
}
