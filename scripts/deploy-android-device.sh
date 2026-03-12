#!/bin/bash

# Build and deploy KidsIdKit to a connected Android device
# Usage: ./scripts/deploy-android-device.sh

set -e

# Change to repo root for consistent relative paths
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/.."

# Resolve dotnet executable
source "$SCRIPT_DIR/dotnet-resolve.sh"

echo "Deploying KidsIdKit to Android device..."
echo "Make sure your device is:"
echo "  - Connected via USB"
echo "  - Unlocked"
echo "  - Has USB Debugging enabled (Settings → Developer Options)"
echo ""

# Check for connected devices
echo "Checking for connected devices..."
if ! command -v adb &> /dev/null; then
  echo "❌ Error: adb not found. Ensure Android SDK platform-tools are installed."
  echo "   Set ANDROID_HOME or add platform-tools to PATH."
  exit 1
fi

DEVICES=$(adb devices | grep -v "List of devices" | grep "device$")
if [ -z "$DEVICES" ]; then
  echo "❌ No Android devices connected."
  echo "   Connect your device via USB with USB Debugging enabled."
  exit 1
fi

echo "Found device(s):"
adb devices | grep -v "List of devices" | grep -v "^$" | sed 's/^/  /'
echo ""

echo "Building and deploying..."
"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-android

echo "✅ Deployment complete!"
