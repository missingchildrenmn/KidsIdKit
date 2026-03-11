#!/bin/bash

# Deploy KidsIdKit to connected iOS device
# Usage: ./scripts/deploy-ios-device.sh [device-name]
# Example: ./scripts/deploy-ios-device.sh "Perry iPhone"

set -e

# Resolve dotnet executable
source "$(dirname "$0")/dotnet-resolve.sh"

# Get device name from argument (required)
if [ -z "$1" ]; then
  echo "❌ Error: Device name required"
  echo "Usage: $0 \"Device Name\""
  echo ""
  echo "Available devices:"
  xcrun devicectl list devices 2>/dev/null | grep -i "iPhone\|iPad" || echo "  No devices found"
  exit 1
fi

DEVICE_NAME="$1"

echo "Deploying KidsIdKit to device: $DEVICE_NAME"
echo "Make sure your device is:"
echo "  - Connected via USB"
echo "  - Unlocked"
echo "  - Has Developer Mode enabled"
echo ""

# Check if device is connected
echo "Checking for connected devices..."
xcrun devicectl list devices | grep -i "iPhone\|iPad" || {
  echo "❌ No iOS devices found. Please connect your device."
  exit 1
}

echo ""
echo "Building and deploying..."

"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-ios \
  -p:RuntimeIdentifier=ios-arm64 \
  -p:_DeviceName="$DEVICE_NAME"

echo ""
echo "✅ Deployment complete!"
echo "The app should now be running on your device."
