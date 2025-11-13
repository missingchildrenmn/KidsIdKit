# Developer Environment

The app is built as a MAUI Blazor Hybrid app using .NET 9.

Basic requirements:

* A computer that can run .NET and .NET development tools, options include
  * Windows
  * Mac
  * Linux
* Possible development tools
  * Visual Studio 2022
  * Rider
  * VS Code
* .NET 8.0 SDK
* ASP.NET Core web development features/SDK
* MAUI workload (`dotnet workload install maui`)

Here's the structure of the KidsKidKit solution and folders beneath it:

KidsIdKit
├── 📱 Application
│   ├── KidsIdKit.Shared   # Core logic, models, services, and shared components
│   ├── KidsIdKit.Mobile   # .NET MAUI Blazor Hybrid app (main production target)
│   └── KidsIdKit.Web      # Web-only Blazor app for easier testing and debugging
├── KidsIdKit.Tests        # Tests for all the above
