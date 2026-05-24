using Foundation;
using UIKit;

namespace KidsIdKit;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override void OnActivated(UIApplication application)
    {
        UIView.Appearance.TintColor = UIColor.FromRGB(94, 94, 94);
        base.OnActivated(application);
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
