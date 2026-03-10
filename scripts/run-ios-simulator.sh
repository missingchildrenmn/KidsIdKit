#!/bin/bash

# Build and run KidsIdKit in iOS Simulator
# Usage: ./scripts/run-ios-simulator.sh

set -e

echo "Building and running KidsIdKit in iOS Simulator..."

/usr/local/share/dotnet/dotnet build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-ios

echo ""
echo "✅ App should now be running in the iOS Simulator!"
