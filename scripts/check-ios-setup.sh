#!/bin/bash

# Check iOS development environment setup
# Usage: ./scripts/check-ios-setup.sh

echo "Checking iOS Development Environment Setup..."
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

# Check .NET SDK
echo "1. Checking .NET SDK..."
if [ -n "$DOTNET" ] && [ -x "$DOTNET" ]; then
  VERSION=$("$DOTNET" --version)
  echo "   ✅ .NET SDK installed: $VERSION"
  echo "      Path: $DOTNET"
else
  echo "   ❌ .NET SDK not found"
  echo "      Install: brew install --cask dotnet-sdk"
  echo "      Or visit: https://dotnet.microsoft.com/download"
fi
echo ""

# Check MAUI workload
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

# Check Xcode
echo "3. Checking Xcode..."
if command -v xcodebuild &> /dev/null; then
  XCODE_VERSION=$(xcodebuild -version | head -n 1)
  XCODE_PATH=$(xcode-select -p)
  echo "   ✅ Xcode installed: $XCODE_VERSION"
  echo "      Path: $XCODE_PATH"
else
  echo "   ❌ Xcode not found"
  echo "      Install from: https://developer.apple.com/download/"
fi
echo ""

# Check code signing certificates
echo "4. Checking code signing certificates..."
CERT_COUNT=$(security find-identity -v -p codesigning 2>/dev/null | grep -c "Apple Development\|Apple Distribution")
if [ "$CERT_COUNT" -gt 0 ]; then
  echo "   ✅ Found $CERT_COUNT code signing certificate(s):"
  security find-identity -v -p codesigning 2>/dev/null | grep "Apple Development\|Apple Distribution" | sed 's/^/      /'
else
  echo "   ⚠️  No code signing certificates found"
  echo "      Generate in Xcode: Create temp project and enable signing"
fi
echo ""

# Check provisioning profiles
echo "5. Checking provisioning profiles..."
PROFILE_DIR="$HOME/Library/MobileDevice/Provisioning Profiles"
if [ -d "$PROFILE_DIR" ]; then
  PROFILE_COUNT=$(ls -1 "$PROFILE_DIR"/*.mobileprovision 2>/dev/null | wc -l)
  if [ "$PROFILE_COUNT" -gt 0 ]; then
    echo "   ✅ Found $PROFILE_COUNT provisioning profile(s)"
  else
    echo "   ⚠️  No provisioning profiles found"
    echo "      Generate in Xcode with matching Bundle ID"
  fi
else
  echo "   ⚠️  Provisioning profiles directory not found"
fi
echo ""

# Check for connected devices
echo "6. Checking for connected iOS devices..."
if command -v xcrun &> /dev/null; then
  DEVICES=$(xcrun devicectl list devices 2>/dev/null | grep -i "iPhone\|iPad" || echo "")
  if [ -n "$DEVICES" ]; then
    echo "   ✅ Connected devices:"
    echo "$DEVICES" | sed 's/^/      /'
  else
    echo "   ⚠️  No iOS devices connected"
    echo "      Connect device via USB for testing"
  fi
else
  echo "   ❌ xcrun not available"
fi
echo ""

echo "================================================"
echo "Setup check complete!"
echo ""
echo "For detailed setup instructions, see: README_IOS.md"
