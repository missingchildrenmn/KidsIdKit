using System.Diagnostics;
using Microsoft.Maui;

namespace KidsIdKit;

public partial class App : Application
{
	public App()
	{
		try
		{
			Debug.WriteLine("🔧 App.xaml.cs: App constructor starting");
			InitializeComponent();
			Debug.WriteLine("🔧 App.xaml.cs: InitializeComponent completed");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"❌ App.xaml.cs: Exception in constructor: {ex.GetType().Name}");
			Debug.WriteLine($"❌ Message: {ex.Message}");
			Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
			throw;
		}
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Debug.WriteLine("🔧 App.xaml.cs: CreateWindow called");
        return new Window(new MainPage());
    }

    protected override void OnStart()
	{
		Debug.WriteLine("🔧 App.xaml.cs: OnStart called");
		base.OnStart();
	}

	protected override void OnResume()
	{
		Debug.WriteLine("🔧 App.xaml.cs: OnResume called");
		base.OnResume();
	}

	protected override void OnSleep()
	{
		Debug.WriteLine("🔧 App.xaml.cs: OnSleep called");
		base.OnSleep();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		try
		{
			Debug.WriteLine("🔧 App.xaml.cs: CreateWindow called");
			// Create and return the window with the app's main page
			return new Window(new MainPage());
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"❌ App.xaml.cs: Exception in CreateWindow: {ex.GetType().Name}");
			Debug.WriteLine($"❌ Message: {ex.Message}");
			Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
			throw;
		}
	}
}
