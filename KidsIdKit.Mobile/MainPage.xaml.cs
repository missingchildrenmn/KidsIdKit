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

                    float density = androidResources.DisplayMetrics.Density;

                    int statusBarHeightPx = 0;
                    int statusBarResId = androidResources.GetIdentifier("status_bar_height", "dimen", "android");
                    if (statusBarResId > 0)
                    {
                        statusBarHeightPx = androidResources.GetDimensionPixelSize(statusBarResId);
                    }

                    int navBarHeightPx = 0;
                    int navBarResId = androidResources.GetIdentifier("navigation_bar_height", "dimen", "android");
                    if (navBarResId > 0)
                    {
                        navBarHeightPx = androidResources.GetDimensionPixelSize(navBarResId);
                    }

                    // Convert pixels to device-independent units (dp)
                    double topPadding = statusBarHeightPx / density + 8;
                    double bottomPadding = navBarHeightPx / density;

                    if (this.Padding.Top != topPadding || this.Padding.Bottom != bottomPadding)
                    {
                        this.Padding = new Thickness(0, topPadding, 0, bottomPadding);
                        Debug.WriteLine($"🔧 MainPage.xaml.cs: Padding set top={topPadding}, bottom={bottomPadding}");
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