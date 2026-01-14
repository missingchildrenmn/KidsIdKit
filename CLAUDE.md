# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

KidsIdKit is a digital kids ID kit application built for Missing Children Minnesota. It allows parents to maintain a digital record of their children's information. The app uses a shared component architecture with Blazor.

## Build and Test Commands

### .NET Commands (from repository root)
```bash
# Build the solution
dotnet build KidsIdKit.sln

# Build specific project
dotnet build KidsIdKit.Core/KidsIdKit.Core.csproj

# Run tests
dotnet test KidsIdKit.Tests/KidsIdKit.Tests.csproj

# Run a specific test
dotnet test KidsIdKit.Tests/KidsIdKit.Tests.csproj --filter "FullyQualifiedName~TestName"

# Run Web project (for browser testing)
dotnet run --project KidsIdKit.Web/KidsIdKit.Web.csproj
```

### JavaScript Tests (from KidsIdKit.Web directory)
```bash
npm test              # Run Jest tests
npm run test:watch    # Watch mode
npm run test:coverage # Coverage reports
```

### MAUI Development
```bash
# Install MAUI workload (one-time setup)
dotnet workload install maui

# Run on specific platform
dotnet build KidsIdKit.Mobile/KidsIdKit.Mobile.csproj -f net9.0-android
dotnet build KidsIdKit.Mobile/KidsIdKit.Mobile.csproj -f net9.0-ios
```

## Architecture

### Project Structure
- **KidsIdKit.Core** (.NET 8) - Shared Razor components, data models, and service interfaces. All UI and business logic lives here.
- **KidsIdKit.Web** (.NET 8) - Blazor WebAssembly host for browser-based testing. Implements platform services for web.
- **KidsIdKit.Mobile** (.NET 9) - .NET MAUI Blazor Hybrid app. The production deployment target. Implements platform services for native.
- **KidsIdKit.Tests** (.NET 8) - Unit tests using xUnit and bUnit for Blazor component testing.

### Shared Component Pattern
Core contains all Razor components and UI. Both Web and Mobile reference Core and provide platform-specific service implementations:

```
Core (UI + Interfaces) ←── Web (Blazor WASM + Web Services)
                       ←── Mobile (MAUI + Native Services)
```

### Key Service Interfaces (KidsIdKit.Core/Services/)
- `IDataAccess` - Data persistence (Get/Save Family data)
- `IFileSaverService` - File save operations
- `IFileSharerService` - File sharing operations
- `ICameraService` - Camera access

Services are registered as Scoped in Program.cs and injected into components.

### Data Model (KidsIdKit.Core/Data/)
The `Family` class is the root data object containing `Child` records. Each child has related entities: `Person`, `CareProvider`, `FamilyMember`, `DistinguishingFeature`, `Photo`, `MedicalNotes`, `PhysicalDetails`, `SocialMediaAccount`.

### Page Organization (KidsIdKit.Core/Pages/)
Pages are organized by feature in subfolders (Child/, CareProviders/, Friends/, etc.). Each page may have:
- `.razor` - Component markup
- `.razor.cs` - Code-behind
- `.razor.scss` - Styles (compiled by DartSassBuilder)

## Code Style

The project uses `.editorconfig` for style enforcement:
- Use `var` for all type declarations
- File-scoped namespaces (`namespace Foo;`)
- Primary constructors preferred
- Interface names prefixed with `I`
- PascalCase for types, methods, properties
- Prefer switch expressions and pattern matching
