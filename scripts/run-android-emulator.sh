#!/bin/bash

# Build and run KidsIdKit in Android Emulator
# Usage: ./scripts/run-android-emulator.sh

set -e

# Change to repo root for consistent relative paths
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/.."

# Resolve dotnet executable
source "$SCRIPT_DIR/dotnet-resolve.sh"

echo "Building and running KidsIdKit in Android Emulator..."
echo "Make sure an Android Emulator is running."
echo ""

"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -t:Run \
  -f net10.0-android \
  -p:RuntimeIdentifier=android-x64

echo "✅ App launched in emulator!"
