# Android Deployment Guide for KidsIdKit

This guide provides step-by-step instructions for building and deploying the KidsIdKit .NET MAUI Blazor app to Android devices and Google Play.

## Quick Start

Already have everything set up? Jump straight to deployment:

```bash
# Deploy to your connected Android device (from project root)
./scripts/deploy-android-device.sh

# Or test in emulator
./scripts/run-android-emulator.sh
```

See [scripts/README.md](scripts/README.md) for all available commands.

---

## Table of Contents

- [Quick Start](#quick-start)
- [Prerequisites](#prerequisites)
- [Initial Setup](#initial-setup)
- [Development Build (Testing on Your Device)](#development-build-testing-on-your-device)
- [Google Play Deployment](#google-play-deployment)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

1. **macOS** (or Windows/Linux - Android development is cross-platform)
2. **Java JDK 17** - Required by Android SDK tooling:
   ```bash
   brew install --cask temurin@17
   ```
3. **Android SDK** - Install via Android command-line tools:
   ```bash
   brew install --cask android-commandlinetools
   ```
4. **.NET 10.0 SDK** - Install via Homebrew:
   ```bash
   brew install --cask dotnet-sdk
   ```
5. **.NET MAUI Workload**:
   ```bash
   sudo dotnet workload install maui
   ```

### Verify Installation

```bash
# Check Java version
java -version
# Should show: openjdk version "17.x.x"

# Check adb version
adb version
# Should show: Android Debug Bridge version x.x.x

# Check .NET version
dotnet --version
# Should show: 10.0.200 or higher

# Check MAUI workload
dotnet workload list
# Should show: maui

# Or run the setup check script
./scripts/check-android-setup.sh
```

**Note:** These commands assume `dotnet` and `adb` are in your PATH. After installing:
- Restart your terminal, or
- Add to PATH manually:
  ```bash
  # Add to ~/.zshrc
  export JAVA_HOME=$(/usr/libexec/java_home -v 17)
  export ANDROID_HOME="$HOME/Library/Android/sdk"
  export PATH="$ANDROID_HOME/platform-tools:$PATH"
  ```
  Then reload: `source ~/.zshrc`

---

## Initial Setup

### Step 1: Install Android SDK Components

After installing the Android command-line tools, install the required SDK packages:

```bash
# Accept all licenses
yes | sdkmanager --licenses

# Install required components
sdkmanager "platform-tools" "platforms;android-36" "platforms;android-35" "build-tools;36.0.0" "build-tools;35.0.0"

# Optional: Install emulator support
sdkmanager "emulator" "system-images;android-35;google_apis;arm64-v8a"
```

### Step 2: Enable Developer Options on Android Device

1. Go to **Settings → About Phone**
2. Tap **Build Number** 7 times rapidly
   - You'll see "You are now a developer!" message
3. Go back to **Settings → Developer Options** (may be under System)
4. Enable **USB Debugging**
5. Connect your device via USB
6. When prompted on device, tap **"Allow USB Debugging"** and check "Always allow from this computer"

### Step 3: Verify Device Connection

```bash
adb devices
```

You should see your device listed as `device` (not `unauthorized`):
```
List of devices attached
ABC123DEF456    device
```

If you see `unauthorized`, check your phone for the USB debugging authorization prompt.

---

## Development Build (Testing on Your Device)

### Using Convenience Scripts (Recommended)

```bash
# Check your Android development environment
./scripts/check-android-setup.sh

# Build for physical Android device
./scripts/build-android-device.sh

# Build and deploy to connected device
./scripts/deploy-android-device.sh

# Build for emulator
./scripts/build-android-emulator.sh

# Build and run in emulator
./scripts/run-android-emulator.sh
```

### Using dotnet Commands Directly

```bash
# Navigate to project root
cd /path/to/KidsIdKit

# Build for physical device (arm64)
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-android \
  -p:RuntimeIdentifier=android-arm64

# Build and deploy to connected device
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-android

# Build for emulator (x64)
dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-android \
  -p:RuntimeIdentifier=android-x64
```

### App Permissions

The app requests the following permissions at runtime:
- **Camera** - To take photos of children for ID records
- **Read Media Images** (Android 13+) - To access photo library
- **Read External Storage** (Android 12 and below) - To access photos

These will be requested the first time the relevant feature is used.

---

## Google Play Deployment

### Prerequisites for Google Play Distribution

1. **Google Play Developer Account** ($25 one-time fee)
   - Register at: https://play.google.com/console/signup
   - If Missing Children Minnesota has an existing Google Play account, request access

2. **Signing Keystore** - A keystore file is required to sign release builds:
   ```bash
   # Generate a keystore (one-time setup - keep this file safe!)
   keytool -genkeypair -v \
     -keystore kidsidkit.keystore \
     -alias kidsidkit \
     -keyalg RSA \
     -keysize 2048 \
     -validity 10000
   ```
   **Important:** Never commit the keystore to git. Store it securely and back it up - losing it means you cannot update the app on Google Play.

### Step 1: Build Release AAB

Set signing environment variables and run the release script:

```bash
export ANDROID_SIGNING_KEY_STORE=/path/to/kidsidkit.keystore
export ANDROID_SIGNING_KEY_ALIAS=kidsidkit
export ANDROID_SIGNING_KEY_PASS=your_key_password
export ANDROID_SIGNING_STORE_PASS=your_store_password

./scripts/build-android-release.sh
```

Or using dotnet directly:

```bash
dotnet publish \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-android \
  -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=/path/to/kidsidkit.keystore \
  -p:AndroidSigningKeyAlias=kidsidkit \
  -p:AndroidSigningKeyPass=your_key_password \
  -p:AndroidSigningStorePass=your_store_password
```

The `.aab` file will be in `KidsIdKit.Mobile/bin/Release/net10.0-android/`.

### Step 2: Upload to Google Play Console

1. Go to [Google Play Console](https://play.google.com/console)
2. Select your app (or create a new one)
3. Go to **Release → Production** (or Internal Testing for initial upload)
4. Click **Create new release**
5. Upload the `.aab` file
6. Fill in release notes
7. Review and submit

---

## Troubleshooting

### "adb: command not found"

Ensure Android SDK platform-tools are installed and in PATH:
```bash
export ANDROID_HOME="$HOME/Library/Android/sdk"
export PATH="$ANDROID_HOME/platform-tools:$PATH"
source ~/.zshrc
```

### "No Android devices connected"

1. Check USB cable is data-capable (not charge-only)
2. Check USB Debugging is enabled on device
3. Run `adb devices` and accept the authorization prompt on the device
4. Try `adb kill-server && adb start-server`

### "Android SDK directory could not be found"

Set `ANDROID_HOME` environment variable:
```bash
export ANDROID_HOME="$HOME/Library/Android/sdk"
```
Or pass it to the build:
```bash
dotnet build ... -p:AndroidSdkDirectory="$HOME/Library/Android/sdk"
```

### "Java not found" / XA5300 error

Install JDK 17 and set JAVA_HOME:
```bash
brew install --cask temurin@17
export JAVA_HOME=$(/usr/libexec/java_home -v 17)
```

### Device shows as "unauthorized"

1. Unlock your phone
2. Check for the USB debugging authorization dialog
3. Tap "Allow" and optionally check "Always allow from this computer"
4. Run `adb devices` again

### Build error: "SDK location not found"

Run the setup check to diagnose:
```bash
./scripts/check-android-setup.sh
```

---

## App Bundle Identifier

The app uses bundle identifier: `com.missingchildrenmn.kidsidkit`

This is configured in [KidsIdKit.Mobile.csproj](KidsIdKit.Mobile/KidsIdKit.Mobile.csproj) and matches the iOS bundle identifier for consistency.

---

## Minimum Android Version

The app targets **Android 7.0 (API 24)** as the minimum version, covering approximately 97% of active Android devices.
