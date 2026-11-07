namespace KidsIdKit.Core.SharedComponents;

public static class RemovalConfirmation
{
    public const string Header = "Confirm removal";

    private const string MessageFormat = "Are you sure you want to remove\n'{0}'?";

    public static string FormatMessage(string? itemName) =>
        string.Format(MessageFormat, itemName);
}
