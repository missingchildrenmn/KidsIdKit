
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace KidsIdKit.Core.SharedComponents;

public partial class EditBool
{
    private Guid Id { get; } = Guid.NewGuid();

    private DotNetObjectReference<EditBool>? objRef;

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

    protected override async Task  OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            objRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("setBoolEventhandler", objRef, Id.ToString());
        }
    }

    [JSInvokable("UpdateBool")]
    public void UpdateBool(bool newValue)
    {
        CurrentValue = newValue;
    }

    public void Dispose() => objRef?.Dispose();
}
