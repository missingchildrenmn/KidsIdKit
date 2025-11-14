# Developer Environment

The app is built as a MAUI Blazor Hybrid app using .NET 8.

Basic requirements:

* A computer that can run .NET and .NET development tools, options include
  * Windows
  * Mac
  * Linux
* Possible development tools
  * Visual Studio 2022 (or 2026 Insiders)
  * Rider
  * VS Code
* .NET 9.0 SDK
* ASP.NET Core web development features/SDK
* MAUI workload (`dotnet workload install maui`)

# Developer Environment

KidsIdKit
├── 📱 Application
│   ├── KidsIdKit.Core		# Core logic, models, services, and shared components
│   ├── KidsIdKit.Mobile	# .NET MAUI Blazor Hybrid app (main production target)
│   └── KidsIdKit.Web		# Web-only Blazor app for easier testing and debugging
├ Solution Items			# Solution Items
└── docs					# Documentation
	└── dev-environment.md	# This file
└ KidsIdKit.Tests			# Unit and integration tests
