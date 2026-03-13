#!/bin/bash

# Build and run KidsIdKit in iOS Simulator
# Usage: ./scripts/run-ios-simulator.sh

set -e

# Resolve dotnet executable
source "$(dirname "$0")/dotnet-resolve.sh"

echo "Building and running KidsIdKit in iOS Simulator..."

"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-ios

echo ""
echo "✅ App should now be running in the iOS Simulator!"
