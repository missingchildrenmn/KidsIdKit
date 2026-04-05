
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public partial class EditBool
{
    private Guid Id { get; } = Guid.NewGuid();
#nullable enable
    protected override bool TryParseValueFromString(string? value, out bool result, out string validationErrorMessage)
    {
        if (Boolean.TryParse(value, out result))
        {
            validationErrorMessage = string.Empty;
            return true;
        }
        else
        {
            validationErrorMessage = "Value must be a boolean";
            return false;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            var objRef = DotNetObjectReference.Create(this);
            JSRuntime.InvokeVoidAsync("setBoolEventhandler", objRef, Id.ToString());
        }
    }

    [JSInvokable("UpdateBool")]
    public void UpdateBool(bool newValue)
    {
        CurrentValue = newValue;
    }
}
