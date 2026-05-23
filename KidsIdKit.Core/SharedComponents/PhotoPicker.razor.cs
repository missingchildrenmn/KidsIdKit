using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public partial class PhotoPicker
{
    [Parameter, EditorRequired] public Photo Photo { get; set; } = default!;
    [Parameter] public EventCallback<Photo> PhotoChanged { get; set; }

    [Parameter] public bool IsSelecting { get; set; }
    [Parameter] public EventCallback<bool> IsSelectingChanged { get; set; }

    [Parameter] public string Label { get; set; } = "Photo";
    [Parameter] public string AltText { get; set; } = "Photo of child";

    /// <summary>
    /// Unique prefix for this picker's <see cref="IPageState"/> entries.
    /// Override only if a single page hosts more than one <c>PhotoPicker</c>.
    /// </summary>
    [Parameter] public string StateKey { get; set; } = "PhotoPicker";

    private string StagedPhotoKey => $"{StateKey}.StagedPhoto";
    private string ErrorTextKey => $"{StateKey}.ErrorText";
    private string BusyKey => $"{StateKey}.Busy";
    private string SelectingKey => $"{StateKey}.IsSelecting";

    private bool _restoredIsSelecting;

    // Backed by IPageState so an in-progress photo selection survives a BlazorWebView
    // re-mount caused by Android tearing the activity down while the camera intent is in
    // the foreground (and any downstream PIN re-entry).
    private Photo _stagedPhoto
    {
        get => PageState.GetStateItem<Photo>(StagedPhotoKey).Value;
        set => PageState.SetStateItem(StagedPhotoKey, value);
    }

    private string? errorText
    {
        get => PageState.GetStateItem<string?>(ErrorTextKey).Value;
        set => PageState.SetStateItem(ErrorTextKey, value);
    }

    private bool _busy
    {
        get => PageState.GetStateItem<bool>(BusyKey).Value;
        set => PageState.SetStateItem(BusyKey, value);
    }

    private bool PhotoExists => !string.IsNullOrWhiteSpace(Photo?.ImageSource);

    protected override void OnInitialized()
    {
        PageState.InitStateItem<Photo>(StagedPhotoKey, new Photo());
        PageState.InitStateItem<string?>(ErrorTextKey, null);
        PageState.InitStateItem<bool>(BusyKey, false);
        PageState.InitStateItem<bool>(SelectingKey, false);
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_restoredIsSelecting)
        {
            return;
        }

        _restoredIsSelecting = true;

        // If the picker was mid-selection when the BlazorWebView was torn down (for example,
        // by Android relaunching the activity after the camera intent), the parent's plain
        // code-behind state has reset to false. Re-publish the persisted value so the parent
        // and picker stay in sync.
        var persisted = PageState.GetStateItem<bool>(SelectingKey).Value;
        if (persisted != IsSelecting)
        {
            IsSelecting = persisted;
            await IsSelectingChanged.InvokeAsync(IsSelecting);
        }
    }

    private Task StartSelecting()
    {
        _stagedPhoto = new Photo();
        errorText = null;
        return SetIsSelecting(true);
    }

    private Task PickNativePhoto() => RunCameraOperationAsync(PhotoService.PickPhotoFromCameraAsync);

    private Task TakeNativePhoto() => RunCameraOperationAsync(PhotoService.TakePhotoFromCameraAsync);

    private async Task RunCameraOperationAsync(Func<Task<Photo?>> operation)
    {
        errorText = string.Empty;
        _busy = true;
        try
        {
            var photo = await operation();
            if (photo != null)
            {
                _stagedPhoto = photo;
            }
        }
        catch (Exception ex)
        {
            errorText = ex.Message;
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task FileSelected(InputFileChangeEventArgs e)
    {
        errorText = string.Empty;
        try
        {
            _stagedPhoto = await PhotoService.CreatePhotoFromBrowserFileAsync(e.File);
        }
        catch (Exception ex)
        {
            errorText = ex.Message;
        }
    }

    private async Task UseFile()
    {
        if (!string.IsNullOrEmpty(_stagedPhoto.ImageSource))
        {
            Photo = _stagedPhoto;
            await PhotoChanged.InvokeAsync(Photo);
            _stagedPhoto = new Photo();
            await SetIsSelecting(false);
        }
    }

    private Task CancelChoice()
    {
        _stagedPhoto = new Photo();
        return SetIsSelecting(false);
    }

    private async Task SetIsSelecting(bool value)
    {
        if (IsSelecting == value)
        {
            return;
        }
 
        IsSelecting = value;
        PageState.SetStateItem(SelectingKey, IsSelecting);
        await IsSelectingChanged.InvokeAsync(IsSelecting);
    }
}