using System.Diagnostics;

namespace KidsIdKit;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		try
		{
			Debug.WriteLine("🔧 MainPage.xaml.cs: MainPage constructor starting");
			InitializeComponent();
			Debug.WriteLine("🔧 MainPage.xaml.cs: InitializeComponent completed");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"❌ MainPage.xaml.cs: Exception in constructor: {ex.GetType().Name}");
			Debug.WriteLine($"❌ Message: {ex.Message}");
			Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
			throw;
		}
	}

    protected override void OnHandlerChanged()
    {
        try
        {
            Debug.WriteLine("🔧 MainPage.xaml.cs: OnHandlerChanged called");
            base.OnHandlerChanged();

#if ANDROID
            PlaceAppBelowAndroidStatusBar();

            void PlaceAppBelowAndroidStatusBar()
            {
                try
                {
                    Debug.WriteLine("🔧 MainPage.xaml.cs: PlaceAppBelowAndroidStatusBar starting");
                    
                    Android.Views.View? mauiView = this.Handler?.PlatformView as Android.Views.View;
                    var androidResources = mauiView?.Context?.Resources;
                    
                    if (mauiView == null || androidResources?.DisplayMetrics == null)
                    {
                        Debug.WriteLine("⚠️ MainPage.xaml.cs: mauiView or androidResources is null");
                        return;
                    }

                    int statusBarHeightPx = 0;
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
                        Debug.WriteLine($"🔧 MainPage.xaml.cs: Padding set to {appBarOffset}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ MainPage.xaml.cs: Exception in PlaceAppBelowAndroidStatusBar: {ex.GetType().Name}");
                    Debug.WriteLine($"❌ Message: {ex.Message}");
                    Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                }
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ MainPage.xaml.cs: Exception in OnHandlerChanged: {ex.GetType().Name}");
            Debug.WriteLine($"❌ Message: {ex.Message}");
            Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
        }
    }
}
