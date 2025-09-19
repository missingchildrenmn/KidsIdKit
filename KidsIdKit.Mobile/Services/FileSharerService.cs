using KidsIdKit.Shared.Services;

namespace KidsIdKit.Mobile.Services;

public class FileSharerService : IFileSharerService
{
    public async Task ShareFileAsync(string filename)
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, filename);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share Child Info",
            File = new ShareFile(path)
        });
    }

    //public async Task ShareFileByEmailAsync(string filename, string subject, string body, string recipient)
    //{
    //    var path = Path.Combine(FileSystem.AppDataDirectory, filename);
    //    var message = new EmailMessage
    //    {
    //        Subject = subject,
    //        Body = body,
    //        To = new List<string> { recipient },
    //        Attachments = { new EmailAttachment(path) }
    //    };

    //    await Email.Default.ComposeAsync(message);
    //}

    // Remove unsupported email sharing for platforms where Email.ComposeAsync is not available.
    // Add platform check to avoid FeatureNotSupportedException.
}
