namespace KidsIdKit;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if ANDROID
        PlaceAppBelowAndroidStatusBar();

        void PlaceAppBelowAndroidStatusBar()
        {
            Android.Views.View? mauiView = this.Handler?.PlatformView as Android.Views.View;
            if (mauiView != null)
            {
                int statusBarHeightPx = 0;
                var androidResources = mauiView.Context.Resources;
                int resourceId = androidResources.GetIdentifier("status_bar_height", "dimen", "android");
                if (resourceId > 0)
                {
                    statusBarHeightPx = androidResources.GetDimensionPixelSize(resourceId);
                }

                // Convert pixels to device-independent units (dp)
                float density = androidResources.DisplayMetrics.Density;
                double statusBarHeightDp = statusBarHeightPx / density;

                // Optionally add a small offset for visual comfort
                double appBarOffset = statusBarHeightDp + 8;

                if (this.Padding.Top != appBarOffset)
                {
                    this.Padding = new Thickness(0, appBarOffset, 0, 0);
                }
            }
        }
#endif
    }
}
