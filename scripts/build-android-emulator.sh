#!/bin/bash

# Build KidsIdKit for Android Emulator
# Usage: ./scripts/build-android-emulator.sh

set -e

# Change to repo root for consistent relative paths
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/.."

# Resolve dotnet executable
source "$SCRIPT_DIR/dotnet-resolve.sh"

echo "Building KidsIdKit for Android Emulator..."

"$DOTNET" build \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-android \
  -p:RuntimeIdentifier=android-x64

echo "✅ Build complete!"
echo "Output: KidsIdKit.Mobile/bin/Debug/net10.0-android/android-x64/"
