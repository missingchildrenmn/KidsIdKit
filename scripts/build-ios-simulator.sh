#!/bin/bash

# Build KidsIdKit for iOS Simulator
# Usage: ./scripts/build-ios-simulator.sh

set -e

# Resolve dotnet executable
source "$(dirname "$0")/dotnet-resolve.sh"

echo "Building KidsIdKit for iOS Simulator..."

"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios

echo "✅ Build complete!"
echo "Output: KidsIdKit.Mobile/bin/Debug/net10.0-ios/iossimulator-arm64/"
