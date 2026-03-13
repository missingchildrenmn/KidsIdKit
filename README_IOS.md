# iOS Deployment Guide for KidsIdKit

This guide provides step-by-step instructions for building and deploying the KidsIdKit .NET MAUI Blazor app to iOS devices and the App Store.

## Quick Start

Already have everything set up? Jump straight to deployment:

```bash
# Deploy to your iPhone (from project root)
./scripts/deploy-ios-device.sh "Your iPhone Name"

# Or test in simulator
./scripts/run-ios-simulator.sh
```

See [scripts/README.md](scripts/README.md) for all available commands.

---

## Table of Contents

- [Quick Start](#quick-start)
- [Prerequisites](#prerequisites)
- [Initial Setup](#initial-setup)
- [Development Build (Testing on Your Device)](#development-build-testing-on-your-device)
- [App Store Deployment](#app-store-deployment)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

1. **macOS** - iOS development requires a Mac
2. **Xcode 26.2** - Download from [Apple Developer](https://developer.apple.com/download/)
3. **.NET 10.0 SDK** - Install via Homebrew:
   ```bash
   brew install --cask dotnet-sdk
   ```
4. **.NET MAUI Workload**:
   ```bash
   sudo dotnet workload install maui
   ```

### Verify Installation

```bash
# Check .NET version
dotnet --version
# Should show: 10.0.200 or higher

# Check MAUI workload
dotnet workload list
# Should show: maui

# Check Xcode version
xcodebuild -version
# Should show: Xcode 26.2
```

**Note:** These commands assume `dotnet` is in your PATH. After installing via Homebrew, you may need to:
- Restart your terminal, or
- Add dotnet to PATH manually:
  ```bash
  # For Homebrew on Apple Silicon
  export PATH="/opt/homebrew/bin:$PATH"

  # For Homebrew on Intel Mac
  export PATH="/usr/local/bin:$PATH"
  ```
- If `dotnet` is not found, locate it with: `which dotnet`

---

## Initial Setup

### 1. iOS Configuration Files

The project already includes the necessary iOS configuration:

- **Info.plist** ([KidsIdKit.Mobile/Platforms/iOS/Info.plist](KidsIdKit.Mobile/Platforms/iOS/Info.plist))
  - Camera usage permission
  - Photo library access permissions

- **Entitlements.plist** ([KidsIdKit.Mobile/Platforms/iOS/Entitlements.plist](KidsIdKit.Mobile/Platforms/iOS/Entitlements.plist))
  - iOS capabilities configuration

- **KidsIdKit.Mobile.csproj** ([KidsIdKit.Mobile/KidsIdKit.Mobile.csproj](KidsIdKit.Mobile/KidsIdKit.Mobile.csproj))
  - Bundle ID: `com.missingchildrenmn.kidsidkit`
  - Code signing configuration

### 2. Xcode Version Compatibility

If you have Xcode 26.3 or newer, you need to use Xcode 26.2 for compatibility with the current .NET MAUI workload:

```bash
# Switch to Xcode 26.2
sudo xcode-select --switch "/Applications/Xcode 26.2 .app"

# Verify the switch
xcodebuild -version
```

---

## Development Build (Testing on Your Device)

### Step 1: Generate Apple Development Certificate

For free development (7-day certificate), you need to generate a development certificate:

1. Open Xcode
2. Create a new iOS project: **File → New → Project → iOS → App**
3. Fill in details:
   - Product Name: `TempApp`
   - Team: Select or add your Apple ID
   - Organization Identifier: `com.yourname`
4. In project settings:
   - Select the target
   - Go to **"Signing & Capabilities"**
   - Check **"Automatically manage signing"**
   - Select your Apple ID as Team

Xcode will automatically generate the development certificate.

5. Verify certificate:
   ```bash
   security find-identity -v -p codesigning
   ```
   You should see: `"Apple Development: Your Name (TEAMID)"`

### Step 2: Create Provisioning Profile

1. In the same TempApp project in Xcode
2. Change the Bundle Identifier to: `com.missingchildrenmn.kidsidkit`
3. Build the project (⌘+B)
4. This creates a provisioning profile for KidsIdKit

5. Copy profiles to standard location:
   ```bash
   cp ~/Library/Developer/Xcode/UserData/Provisioning\ Profiles/*.mobileprovision \
      ~/Library/MobileDevice/Provisioning\ Profiles/
   ```

### Step 3: Update Project Configuration

The project should already be configured with your signing identity in [KidsIdKit.Mobile.csproj](KidsIdKit.Mobile/KidsIdKit.Mobile.csproj):

```xml
<PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net10.0-ios|AnyCPU'">
  <ApplicationId>com.missingchildrenmn.kidsidkit</ApplicationId>
  <CodesignEntitlements>Platforms\iOS\Entitlements.plist</CodesignEntitlements>
  <CodesignKey>Apple Development: Your Name (TEAMID)</CodesignKey>
  <CodesignProvision>Automatic</CodesignProvision>
  <MtouchLink>SdkOnly</MtouchLink>
</PropertyGroup>
```

Update the `CodesignKey` with your certificate name from Step 1.

### Step 4: Enable Developer Mode on iPhone

On iOS 16+, you must enable Developer Mode:

1. On iPhone: **Settings → Privacy & Security → Developer Mode**
2. Toggle **Developer Mode** to ON
3. Restart when prompted
4. After restart, tap **Turn On** and enter passcode

### Step 5: Build and Deploy

#### Option A: Using Convenience Scripts (Recommended)

The project includes helper scripts in the [scripts/](scripts/) directory for common tasks:

```bash
# Check your iOS development environment setup
./scripts/check-ios-setup.sh

# Build for iOS Simulator
./scripts/build-ios-simulator.sh

# Build and run in iOS Simulator
./scripts/run-ios-simulator.sh

# Build for physical device
./scripts/build-ios-device.sh

# Deploy to connected iPhone (device name required)
./scripts/deploy-ios-device.sh "Your iPhone Name"

# List available devices to find your device name
xcrun devicectl list devices

# Clean build artifacts
./scripts/clean-ios.sh
```

See [scripts/README.md](scripts/README.md) for detailed documentation on all available scripts.

**Important Notes:**
- Make sure your iPhone is connected via USB, unlocked, and has Developer Mode enabled
- Free development certificates expire after 7 days. You'll need to rebuild and redeploy after this period
- Get your exact device name from: `xcrun devicectl list devices`

#### Option B: Using dotnet Commands Directly

```bash
# Navigate to project root
cd /path/to/KidsIdKit

# Build for iOS Simulator (testing without device)
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios

# Build for physical device
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios \
  -p:RuntimeIdentifier=ios-arm64

# Deploy to connected iPhone
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-ios \
  -p:RuntimeIdentifier=ios-arm64 \
  -p:_DeviceName="Your iPhone Name"
```

---

## App Store Deployment

### Prerequisites for App Store Distribution

1. **Apple Developer Program Membership** ($99/year)
   - Organization account (for Missing Children Minnesota)
   - Visit: https://developer.apple.com/programs/enroll/

2. **Team Access**
   - If Missing Children Minnesota has an Apple Developer account, request to be added as:
     - **App Manager** (can manage apps, TestFlight, App Store submissions)
     - **Developer** (can create certificates, test on devices)

### Step 1: Register App ID

1. Sign in to [Apple Developer Portal](https://developer.apple.com/account/)
2. Navigate to **Certificates, Identifiers & Profiles**
3. Click **Identifiers** → **+** button
4. Select **App IDs** → **App**
5. Configure:
   - Description: `Kids ID Kit`
   - Bundle ID: `com.missingchildrenmn.kidsidkit` (Explicit)
   - Capabilities: Enable any needed (Camera, Photo Library are implicit)
6. Click **Continue** → **Register**

### Step 2: Create Distribution Certificate

1. In **Certificates, Identifiers & Profiles** → **Certificates**
2. Click **+** button
3. Select **Apple Distribution**
4. Follow prompts to create Certificate Signing Request (CSR)
5. Upload CSR and download certificate
6. Double-click downloaded certificate to install in Keychain

### Step 3: Create Distribution Provisioning Profile

1. In **Certificates, Identifiers & Profiles** → **Profiles**
2. Click **+** button
3. Select **App Store** under Distribution
4. Select App ID: `com.missingchildrenmn.kidsidkit`
5. Select Distribution certificate
6. Download and install profile:
   ```bash
   cp ~/Downloads/*.mobileprovision \
      ~/Library/MobileDevice/Provisioning\ Profiles/
   ```

### Step 4: Configure Release Build

Update [KidsIdKit.Mobile.csproj](KidsIdKit.Mobile/KidsIdKit.Mobile.csproj) Release configuration:

```xml
<PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net10.0-ios|AnyCPU'">
  <ApplicationId>com.missingchildrenmn.kidsidkit</ApplicationId>
  <CodesignEntitlements>Platforms\iOS\Entitlements.plist</CodesignEntitlements>
  <CodesignKey>Apple Distribution: Missing Children Minnesota (TEAMID)</CodesignKey>
  <CodesignProvision>KidsIdKit Distribution Profile</CodesignProvision>
  <MtouchLink>Full</MtouchLink>
  <EnableAssemblyILStripping>true</EnableAssemblyILStripping>
  <MtouchUseLlvm>true</MtouchUseLlvm>
  <MtouchFloat32>true</MtouchFloat32>
</PropertyGroup>
```

### Step 5: Build Release Archive

```bash
# Clean previous builds
dotnet clean

# Build Release for App Store
dotnet publish \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios \
  -c Release \
  -p:ArchiveOnBuild=true \
  -p:RuntimeIdentifier=ios-arm64
```

The archive will be created at:
```
KidsIdKit.Mobile/bin/Release/net10.0-ios/ios-arm64/publish/
```

### Step 6: Create App Store Connect Record

1. Sign in to [App Store Connect](https://appstoreconnect.apple.com/)
2. Click **My Apps** → **+** → **New App**
3. Configure:
   - Platform: **iOS**
   - Name: **Kids ID Kit**
   - Primary Language: **English (U.S.)**
   - Bundle ID: Select `com.missingchildrenmn.kidsidkit`
   - SKU: `kidsidkit-ios`
   - User Access: **Full Access**
4. Click **Create**

### Step 7: Upload Build to App Store Connect

You can upload using Xcode or Transporter:

**Using Xcode:**
1. Open Xcode
2. Window → Organizer
3. Select the archive
4. Click **Distribute App**
5. Select **App Store Connect** → **Upload**
6. Follow prompts

**Using Transporter:**
1. Download [Transporter](https://apps.apple.com/us/app/transporter/id1450874784) from Mac App Store
2. Build the .ipa:
   ```bash
   dotnet publish \
     KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
     -f net10.0-ios \
     -c Release \
     -p:RuntimeIdentifier=ios-arm64 \
     -p:BuildIpa=true
   ```
   The .ipa will be created in: `KidsIdKit.Mobile/bin/Release/net10.0-ios/ios-arm64/publish/`
3. Open Transporter and drag the .ipa file

### Step 8: Prepare App Store Listing

In App Store Connect, complete the following sections:

**App Information:**
- Privacy Policy URL
- Category: **Medical** or **Lifestyle**
- Content Rights: As appropriate

**Pricing and Availability:**
- Price: **Free**
- Availability: Select countries

**App Privacy:**
- Data Types Collected:
  - Photos (for ID records)
  - User Content (child information)
- Data Use: Explain it's stored locally and encrypted

**Version Information:**
- Screenshots (required for 6.7", 6.5", 5.5" displays)
  - Use iOS Simulator to capture screenshots at different sizes
- Description:
  ```
  Kids ID Kit is a digital kids ID kit application for Missing Children Minnesota.
  It allows parents to maintain a secure, encrypted digital record of their children's
  information including photos, physical details, and emergency contacts.

  Features:
  • Secure, encrypted local storage
  • Photo management for child ID records
  • Physical characteristics tracking
  • Emergency contact information
  • Medical notes and details
  • Social media account tracking
  • Export capabilities for sharing with authorities if needed
  ```
- Keywords: `child safety, id kit, missing children, emergency contacts`
- Support URL: https://github.com/missingchildrenmn/KidsIdKit
- Marketing URL: https://missingchildrenmn.org (if available)

**Build:**
- Select the uploaded build

**App Review Information:**
- Contact information for app review team
- Notes: Explain the app's purpose and test instructions

### Step 9: Submit for Review

1. In App Store Connect, ensure all sections have green checkmarks
2. Click **Add for Review**
3. Click **Submit to App Review**

**Review Timeline:** Typically 1-3 days

---

## Troubleshooting

### Build Issues

**Error: "This version of .NET for iOS requires Xcode 26.2"**
- Solution: Switch to Xcode 26.2 (see [Xcode Version Compatibility](#2-xcode-version-compatibility))

**Error: "No valid iOS code signing keys found"**
- Solution: Generate Apple Development certificate (see [Step 1: Generate Apple Development Certificate](#step-1-generate-apple-development-certificate))

**Error: "Could not find any available provisioning profiles"**
- Solution: Create provisioning profile in Xcode (see [Step 2: Create Provisioning Profile](#step-2-create-provisioning-profile))

### Deployment Issues

**Error: "Developer Mode is disabled"**
- Solution: Enable Developer Mode on iPhone (see [Step 4: Enable Developer Mode](#step-4-enable-developer-mode-on-iphone))

**Error: "Unable to install app"**
- Check iPhone is unlocked
- Trust computer on iPhone when prompted
- Verify USB connection
- Try: Settings → General → VPN & Device Management → Trust developer

**App crashes on launch:**
- Check Console app on Mac for crash logs
- Common issues:
  - Missing entitlements
  - Privacy permissions not set in Info.plist
  - Trimming removed required code (adjust `MtouchLink` setting)

### Certificate Issues

**Certificate expired:**
- Free certificates expire after 7 days
- Paid Apple Developer certificates expire after 1 year
- Regenerate certificate and rebuild app

**Provisioning profile expired:**
- Regenerate in Apple Developer portal or Xcode
- Copy new profile to `~/Library/MobileDevice/Provisioning Profiles/`
- Rebuild app

---

## Quick Reference Commands

```bash
# Build for Simulator
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj -f net10.0-ios

# Build for Device
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios -p:RuntimeIdentifier=ios-arm64

# Deploy to Device
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj -t:Run \
  -f net10.0-ios -p:RuntimeIdentifier=ios-arm64 \
  -p:_DeviceName="iPhone Name"

# Release Build
dotnet publish \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios -c Release -p:RuntimeIdentifier=ios-arm64

# Clean Build
dotnet clean

# List connected devices
xcrun devicectl list devices

# Check code signing identity
security find-identity -v -p codesigning

# List provisioning profiles
ls -la ~/Library/MobileDevice/Provisioning\ Profiles/
```

---

## Additional Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Apple Developer Documentation](https://developer.apple.com/documentation/)
- [App Store Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [Missing Children Minnesota](https://missingchildrenmn.org/)
- [KidsIdKit GitHub Repository](https://github.com/missingchildrenmn/KidsIdKit)
- [KidsIdKit Discord](https://discord.gg/ybzhYHBM2e)

---

## Contact

For questions or issues:
- Create an issue on [GitHub](https://github.com/missingchildrenmn/KidsIdKit/issues)
- Join the [KidsIdKit Discord server](https://discord.gg/ybzhYHBM2e)

---

**Last Updated:** March 10, 2026
**Project Version:** 1.0
**.NET Version:** 10.0.200
**Xcode Version:** 26.2
