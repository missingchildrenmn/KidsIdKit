#!/bin/bash

# Clean iOS build artifacts
# Usage: ./scripts/clean-ios.sh

set -e

# Resolve dotnet executable
source "$(dirname "$0")/dotnet-resolve.sh"

echo "Cleaning iOS build artifacts..."

"$DOTNET" clean KidsIdKit.Mobile/KidsIdKit.Mobile.csproj

# Remove obj and bin directories
rm -rf KidsIdKit.Mobile/obj
rm -rf KidsIdKit.Mobile/bin
rm -rf KidsIdKit.Core/obj
rm -rf KidsIdKit.Core/bin

echo "✅ Clean complete!"
