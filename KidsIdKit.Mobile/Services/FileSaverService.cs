using KidsIdKit.Shared.Services;

namespace KidsIdKit.Mobile.Services;

public class FileSaverService : IFileSaverService
{
    // Remove unsupported email sharing for platforms where Email.ComposeAsync is not available.
    // Add platform check to avoid FeatureNotSupportedException.

    public async Task SaveFileAsync(string filename, string content)
    {
        try
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, filename);
            Console.WriteLine($"Saving file to: {path}");
            await File.WriteAllTextAsync(path, content);
            Console.WriteLine("File saved successfully.");
            var exists = File.Exists(path);
            Console.WriteLine($"File exists after write: {exists}");

            await ShareFileAsync(path);

            //// Only attempt to share file by email if supported
            //if (Email.Default.IsComposeSupported)
            //{
            //    //await ShareFileByEmailAsync(filename, "Missing Child - <Name>", "Attached, please find all the information on the missing child, <Name>", "");
            //}
            //else
            //{
            //    Console.WriteLine("Email compose is not supported on this platform.");
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex}");
        }
    }

    // TODO: Move this to a separate IFileSharingService.cs/FileSharingService.cs
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
}