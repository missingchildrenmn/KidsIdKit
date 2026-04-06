using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.SharedComponents;

public partial class McmAlert
{
    [Parameter]
    public string Header { get; set; } = string.Empty;
    [Parameter]
    public string Message { get; set; } = string.Empty;
    [Parameter]
    public string ConfirmPrompt { get; set; } = string.Empty;
    [Parameter]
    public string CancelPrompt { get; set; } = string.Empty;
    [Parameter]
    public bool Show { get; set; } = false;
    [Parameter]
    public string StateInformation { get; set; } = string.Empty;
    [Parameter]
    public EventCallback<(AlertAction action, string stateInformation)> AlertClosed { get; set; }

    private Guid Id { get; } = Guid.NewGuid();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            var objRef = DotNetObjectReference.Create(this);
            JSRuntime.InvokeVoidAsync("setAlertEventhandler", objRef, Id.ToString(), ConfirmPrompt, CancelPrompt);
        }
    }

    [JSInvokable("AlertClosedCallback")]
    public void AlertClosedCallback(string action)
    {
        if (AlertClosed.HasDelegate)
        {
            AlertAction returnValue = AlertAction.Cancel;
            if (action == "confirm")
            {
                returnValue = AlertAction.Confirm;
            }
            AlertClosed.InvokeAsync((returnValue, StateInformation));
        }


    }

    public enum AlertAction
    {
        Confirm,
        Cancel
    }
}
