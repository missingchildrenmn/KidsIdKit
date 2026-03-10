#!/bin/bash

# Build KidsIdKit for iOS Simulator
# Usage: ./scripts/build-ios-simulator.sh

set -e

echo "Building KidsIdKit for iOS Simulator..."

/usr/local/share/dotnet/dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios

echo "✅ Build complete!"
echo "Output: KidsIdKit.Mobile/bin/Debug/net10.0-ios/iossimulator-arm64/"
