# Build Scripts

This directory contains helper scripts for building and deploying the KidsIdKit iOS and Android applications.

## Available Scripts

### Android Scripts

#### `check-android-setup.sh`
Verifies your Android development environment is properly configured.

```bash
./scripts/check-android-setup.sh
```

**Checks:**
- .NET SDK installation
- MAUI workload installation
- Java JDK installation
- Android SDK installation
- adb (Android Debug Bridge)
- Connected Android devices

---

#### `build-android-device.sh`
Builds the app for a physical Android device (arm64).

```bash
./scripts/build-android-device.sh
```

**Output:** `KidsIdKit.Mobile/bin/Debug/net10.0-android/android-arm64/`

---

#### `deploy-android-device.sh`
Builds and deploys the app to a connected Android device.

```bash
./scripts/deploy-android-device.sh
```

**Requirements:**
- Android device connected via USB
- USB Debugging enabled on device
- Device authorized (accepted "Allow USB Debugging" prompt)

---

#### `build-android-emulator.sh`
Builds the app for Android Emulator (x64).

```bash
./scripts/build-android-emulator.sh
```

**Output:** `KidsIdKit.Mobile/bin/Debug/net10.0-android/android-x64/`

---

#### `run-android-emulator.sh`
Builds and launches the app in a running Android Emulator.

```bash
./scripts/run-android-emulator.sh
```

**Requirements:** An Android Emulator must be running before executing this script.

---

#### `build-android-release.sh`
Builds a signed Release AAB for Google Play Store submission.

```bash
export ANDROID_SIGNING_KEY_STORE=/path/to/kidsidkit.keystore
export ANDROID_SIGNING_KEY_ALIAS=kidsidkit
export ANDROID_SIGNING_KEY_PASS=your_key_password
export ANDROID_SIGNING_STORE_PASS=your_store_password

./scripts/build-android-release.sh
```

**Output:** `KidsIdKit.Mobile/bin/Release/net10.0-android/`

**Requirements:**
- Signing keystore file (generate with `keytool`)
- Signing environment variables set

---

### iOS Scripts

### Development Scripts

#### `check-ios-setup.sh`
Verifies your iOS development environment is properly configured.

```bash
./scripts/check-ios-setup.sh
```

**Checks:**
- .NET SDK installation
- MAUI workload installation
- Xcode installation and version
- Code signing certificates
- Provisioning profiles
- Connected iOS devices

---

#### `build-ios-simulator.sh`
Builds the app for iOS Simulator (for testing without a physical device).

```bash
./scripts/build-ios-simulator.sh
```

**Output:** `KidsIdKit.Mobile/bin/Debug/net10.0-ios/iossimulator-arm64/`

---

#### `run-ios-simulator.sh`
Builds and launches the app in iOS Simulator.

```bash
./scripts/run-ios-simulator.sh
```

The iOS Simulator will launch automatically and the app will install and run.

---

#### `build-ios-device.sh`
Builds the app for a physical iOS device.

```bash
./scripts/build-ios-device.sh
```

**Output:** `KidsIdKit.Mobile/bin/Debug/net10.0-ios/ios-arm64/`

**Requirements:**
- Apple Development certificate installed
- Provisioning profile for `com.missingchildrenmn.kidsidkit`
- Device connected via USB

---

#### `deploy-ios-device.sh`
Builds and deploys the app to a connected iOS device.

```bash
# Deploy to specified device (device name required)
./scripts/deploy-ios-device.sh "My iPhone"

# List available devices
xcrun devicectl list devices
```

**Requirements:**
- iOS device connected via USB
- Device unlocked
- Developer Mode enabled on device
- Device trusted on Mac

**Note:** Free development certificates expire after 7 days. You'll need to rebuild and redeploy.

---

### Release Scripts

#### `build-ios-release.sh`
Builds a Release version for App Store submission.

```bash
./scripts/build-ios-release.sh
```

**Output:** `KidsIdKit.Mobile/bin/Release/net10.0-ios/ios-arm64/publish/`

**Requirements:**
- Apple Distribution certificate installed
- Distribution provisioning profile
- Updated `CodesignKey` in `KidsIdKit.Mobile.csproj` Release configuration

**Next Steps After Build:**
1. Open Xcode Organizer (Window → Organizer)
2. Or use Transporter app to upload to App Store Connect

---

### Utility Scripts

#### `clean-ios.sh`
Cleans all iOS build artifacts and intermediate files.

```bash
./scripts/clean-ios.sh
```

Removes:
- `obj/` directories
- `bin/` directories
- Build cache

Use this when you encounter build issues or want a fresh build.

---

## Quick Reference

### Android
```bash
# Check setup
./scripts/check-android-setup.sh

# Test on Device
./scripts/deploy-android-device.sh

# Test in Emulator
./scripts/run-android-emulator.sh

# Build for Google Play
./scripts/build-android-release.sh
```

### iOS
```bash
# Check setup
./scripts/check-ios-setup.sh

# Test in Simulator
./scripts/run-ios-simulator.sh

# Test on Device
./scripts/deploy-ios-device.sh "Your iPhone"

# Clean build
./scripts/clean-ios.sh

# Build for App Store
./scripts/build-ios-release.sh
```

---

## Troubleshooting

### Script Permission Denied

If you get a permission error:

```bash
chmod +x scripts/*.sh
```

### Device Not Found

If deploy script can't find your device:

1. Check device is connected via USB
2. Unlock the device
3. Trust the computer (prompt on device)
4. Check device name matches:
   ```bash
   xcrun devicectl list devices
   ```

### Build Failures

1. Check your environment:
   ```bash
   ./scripts/check-ios-setup.sh
   ```

2. Try a clean build:
   ```bash
   ./scripts/clean-ios.sh
   ./scripts/build-ios-device.sh
   ```

3. Verify Xcode version:
   ```bash
   xcodebuild -version
   ```
   Should show: Xcode 26.2

### Certificate Issues

If you see certificate/signing errors:

1. Verify certificates exist:
   ```bash
   security find-identity -v -p codesigning
   ```

2. Check provisioning profiles:
   ```bash
   ls -la ~/Library/MobileDevice/Provisioning\ Profiles/
   ```

3. Regenerate in Xcode:
   - Create temp project
   - Change Bundle ID to `com.missingchildrenmn.kidsidkit`
   - Build to generate profile

---

## Additional Resources

- **iOS Setup Guide:** [README_IOS.md](../README_IOS.md)
- **Android Setup Guide:** [README_ANDROID.md](../README_ANDROID.md)
- **Project Documentation:** [readme.md](../readme.md)
- **GitHub Issues:** https://github.com/missingchildrenmn/KidsIdKit/issues
- **Discord:** https://discord.gg/ybzhYHBM2e

---

## Script Maintenance

### Dotnet Resolution

These scripts automatically find the `dotnet` executable using `dotnet-resolve.sh`, which checks:
1. `$DOTNET_ROOT` environment variable (if set)
2. `dotnet` in your `$PATH`
3. Common installation locations:
   - `/usr/local/share/dotnet/dotnet` (Homebrew Intel Mac)
   - `$HOME/.dotnet/dotnet` (manual install)

If dotnet is not found, the scripts will display installation instructions.

**Custom dotnet location:**
Set the `DOTNET_ROOT` environment variable:
```bash
export DOTNET_ROOT=/path/to/dotnet
./scripts/build-ios-simulator.sh
```

Or add to your shell profile (`~/.zshrc` or `~/.bash_profile`):
```bash
export DOTNET_ROOT=/path/to/dotnet
export PATH="$DOTNET_ROOT:$PATH"
```

---

**Last Updated:** March 10, 2026
