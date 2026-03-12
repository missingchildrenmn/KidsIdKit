#!/bin/bash

# Build KidsIdKit Release AAB for Google Play Store
# Usage: ./scripts/build-android-release.sh
#
# Prerequisites:
#   - Set ANDROID_SIGNING_KEY_STORE to path of your keystore file
#   - Set ANDROID_SIGNING_KEY_ALIAS to your key alias
#   - Set ANDROID_SIGNING_KEY_PASS to your key password
#   - Set ANDROID_SIGNING_STORE_PASS to your keystore password
#
# Generate a keystore (one-time setup):
#   keytool -genkeypair -v -keystore kidsidkit.keystore -alias kidsidkit \
#     -keyalg RSA -keysize 2048 -validity 10000

set -e

# Change to repo root for consistent relative paths
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/.."

# Resolve dotnet executable
source "$SCRIPT_DIR/dotnet-resolve.sh"

# Validate signing environment variables
if [ -z "${ANDROID_SIGNING_KEY_STORE:-}" ]; then
  echo "❌ Error: ANDROID_SIGNING_KEY_STORE is not set"
  echo "   Export the path to your keystore file:"
  echo "   export ANDROID_SIGNING_KEY_STORE=/path/to/kidsidkit.keystore"
  exit 1
fi

if [ -z "${ANDROID_SIGNING_KEY_ALIAS:-}" ]; then
  echo "❌ Error: ANDROID_SIGNING_KEY_ALIAS is not set"
  exit 1
fi

if [ -z "${ANDROID_SIGNING_KEY_PASS:-}" ]; then
  echo "❌ Error: ANDROID_SIGNING_KEY_PASS is not set"
  exit 1
fi

if [ -z "${ANDROID_SIGNING_STORE_PASS:-}" ]; then
  echo "❌ Error: ANDROID_SIGNING_STORE_PASS is not set"
  exit 1
fi

echo "Building KidsIdKit Release AAB for Google Play..."

"$DOTNET" publish \
  KidsIdKit.Mobile/KidsIdKit.Mobile.csproj \
  -f net10.0-android \
  -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore="$ANDROID_SIGNING_KEY_STORE" \
  -p:AndroidSigningKeyAlias="$ANDROID_SIGNING_KEY_ALIAS" \
  -p:AndroidSigningKeyPass="$ANDROID_SIGNING_KEY_PASS" \
  -p:AndroidSigningStorePass="$ANDROID_SIGNING_STORE_PASS"

echo "✅ Release AAB built!"
echo "Output: KidsIdKit.Mobile/bin/Release/net10.0-android/"
echo ""
echo "Upload the .aab file to Google Play Console."
