#!/bin/bash

# Build KidsIdKit for physical iOS device
# Usage: ./scripts/build-ios-device.sh

set -e

echo "Building KidsIdKit for iOS device..."

/usr/local/share/dotnet/dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios \
  -p:RuntimeIdentifier=ios-arm64

echo "✅ Build complete!"
echo "Output: KidsIdKit.Mobile/bin/Debug/net10.0-ios/ios-arm64/"
