

using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.SharedComponents;


public partial class BusyIndicator
{

    [Parameter]
    public string Id { get; set; } = string.Empty;
    [Parameter]
    public string Message { get; set; } = string.Empty;
    [Parameter]
    public bool Show { get; set; } = false;

    protected override Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.NewGuid().ToString();
        }
        return base.OnParametersSetAsync();
    }
}
