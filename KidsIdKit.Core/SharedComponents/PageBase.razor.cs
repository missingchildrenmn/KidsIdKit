using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class PageBase
{
    public abstract string MenuBarTitle { get; protected set; }

    protected async Task NavigateBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
