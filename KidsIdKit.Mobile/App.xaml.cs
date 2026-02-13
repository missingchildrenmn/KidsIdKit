using System.Diagnostics;

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

			MainPage = new MainPage();
			Debug.WriteLine("🔧 App.xaml.cs: MainPage assigned successfully");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"❌ App.xaml.cs: Exception in constructor: {ex.GetType().Name}");
			Debug.WriteLine($"❌ Message: {ex.Message}");
			Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
			throw;
		}
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
}
