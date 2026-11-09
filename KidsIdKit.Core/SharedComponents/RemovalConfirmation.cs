namespace KidsIdKit.Core.SharedComponents;

public static class RemovalConfirmation
{
    public const string AlertShowState = "AlertShow";
    public const string AlertTitleState = "AlertTitle";
    public const string AlertStateInformationState = "AlertStateInformation";
    public const string AlertMessageState = "AlertMessage";
    public const string Header = "Confirm removal";

    private const string MessageFormat = "Are you sure you want to remove\n'{0}'?";

    public static string FormatMessage(string? itemName) =>
        string.Format(MessageFormat, itemName);
}
