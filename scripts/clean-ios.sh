#!/bin/bash

# Clean iOS build artifacts
# Usage: ./scripts/clean-ios.sh

set -e

# Change to repo root (parent of scripts directory)
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/.."

# Resolve dotnet executable
source "$SCRIPT_DIR/dotnet-resolve.sh"

echo "Cleaning iOS build artifacts..."

"$DOTNET" clean KidsIdKit.Mobile/KidsIdKit.Mobile.csproj

# Remove obj and bin directories
rm -rf KidsIdKit.Mobile/obj
rm -rf KidsIdKit.Mobile/bin
rm -rf KidsIdKit.Core/obj
rm -rf KidsIdKit.Core/bin

echo "✅ Clean complete!"
