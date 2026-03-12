#!/bin/bash

# Check Android development environment setup
# Usage: ./scripts/check-android-setup.sh

echo "Checking Android Development Environment Setup..."
echo ""
echo "================================================"

# Resolve dotnet executable (without exiting on failure)
if [ -n "${DOTNET_ROOT:-}" ]; then
  DOTNET="${DOTNET_ROOT%/}/dotnet"
elif command -v dotnet &> /dev/null; then
  DOTNET="dotnet"
elif [ -x "/usr/local/share/dotnet/dotnet" ]; then
  DOTNET="/usr/local/share/dotnet/dotnet"
elif [ -x "$HOME/.dotnet/dotnet" ]; then
  DOTNET="$HOME/.dotnet/dotnet"
else
  DOTNET=""
fi

# 1. Check .NET SDK
echo "1. Checking .NET SDK..."
if [ -n "$DOTNET" ] && [ -x "$DOTNET" ]; then
  VERSION=$("$DOTNET" --version)
  echo "   ✅ .NET SDK installed: $VERSION"
  echo "      Path: $DOTNET"
else
  echo "   ❌ .NET SDK not found"
  echo "      Install: brew install --cask dotnet-sdk"
fi
echo ""

# 2. Check MAUI workload
echo "2. Checking MAUI workload..."
if [ -n "$DOTNET" ] && [ -x "$DOTNET" ]; then
  if "$DOTNET" workload list 2>/dev/null | grep -q "maui"; then
    echo "   ✅ MAUI workload installed"
  else
    echo "   ❌ MAUI workload not installed"
    echo "      Install: sudo $DOTNET workload install maui"
  fi
else
  echo "   ⚠️  Cannot check (dotnet not found)"
fi
echo ""

# 3. Check Java
echo "3. Checking Java JDK..."
if command -v java &> /dev/null && java -version 2>&1 | grep -q "version"; then
  JAVA_VERSION=$(java -version 2>&1 | head -n 1)
  echo "   ✅ Java installed: $JAVA_VERSION"
  echo "      Path: $(which java)"
else
  echo "   ❌ Java not found"
  echo "      Install: brew install --cask temurin@17"
fi
echo ""

# 4. Check Android SDK
echo "4. Checking Android SDK..."
ANDROID_SDK="${ANDROID_HOME:-$HOME/Library/Android/sdk}"
if [ -d "$ANDROID_SDK/platform-tools" ]; then
  echo "   ✅ Android SDK found: $ANDROID_SDK"
else
  echo "   ❌ Android SDK not found"
  echo "      Install: brew install --cask android-commandlinetools"
  echo "      Then run: sdkmanager \"platform-tools\" \"platforms;android-35\" \"build-tools;35.0.0\""
fi
echo ""

# 5. Check adb
echo "5. Checking adb (Android Debug Bridge)..."
if command -v adb &> /dev/null; then
  ADB_VERSION=$(adb version | head -n 1)
  echo "   ✅ adb installed: $ADB_VERSION"
else
  echo "   ❌ adb not found"
  echo "      Ensure Android SDK platform-tools are installed and ANDROID_HOME is set"
fi
echo ""

# 6. Check connected Android devices
echo "6. Checking for connected Android devices..."
if command -v adb &> /dev/null; then
  DEVICES=$(adb devices | grep -v "List of devices" | grep "device$" || echo "")
  if [ -n "$DEVICES" ]; then
    echo "   ✅ Connected devices:"
    adb devices | grep -v "List of devices" | grep -v "^$" | sed 's/^/      /'
  else
    echo "   ⚠️  No Android devices connected"
    echo "      Connect device via USB with USB Debugging enabled"
  fi
else
  echo "   ⚠️  Cannot check (adb not found)"
fi
echo ""

echo "================================================"
echo "Setup check complete!"
echo ""
echo "For detailed setup instructions, see: README_ANDROID.md"
