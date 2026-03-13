#!/bin/bash

# Build KidsIdKit for iOS App Store Release
# Usage: ./scripts/build-ios-release.sh

set -e

# Resolve dotnet executable
source "$(dirname "$0")/dotnet-resolve.sh"

echo "Building KidsIdKit Release for App Store..."
echo ""
echo "⚠️  Make sure you have:"
echo "  - Apple Distribution certificate installed"
echo "  - Distribution provisioning profile installed"
echo "  - Updated CodesignKey in KidsIdKit.Mobile.csproj"
echo ""
read -p "Press Enter to continue or Ctrl+C to cancel..."

echo ""
echo "Cleaning previous builds..."
"$DOTNET" clean KidsIdKit.Mobile/KidsIdKit.Mobile.csproj

echo ""
echo "Building Release..."
"$DOTNET" publish \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-ios \
  -c Release \
  -p:RuntimeIdentifier=ios-arm64 \
  -p:ArchiveOnBuild=true

echo ""
echo "✅ Release build complete!"
echo "Output: KidsIdKit.Mobile/bin/Release/net10.0-ios/ios-arm64/publish/"
echo ""
echo "Next steps:"
echo "  1. Open Xcode Organizer (Window → Organizer)"
echo "  2. Or use Transporter app to upload the .ipa to App Store Connect"
