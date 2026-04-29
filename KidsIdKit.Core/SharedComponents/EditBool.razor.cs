using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public partial class EditBool
{
    private Guid Id { get; } = Guid.NewGuid();

    private DotNetObjectReference<EditBool>? objRef;

    [Parameter] public EventCallback<bool> OnValueChanged { get; set; }

    [Parameter] public bool Disabled { get; set; }

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
    public async Task UpdateBool(bool newValue)
    {
        CurrentValue = newValue;
        if (OnValueChanged.HasDelegate)
        {
            await OnValueChanged.InvokeAsync(newValue);
        }
    }

    public void Dispose() => objRef?.Dispose();
}
