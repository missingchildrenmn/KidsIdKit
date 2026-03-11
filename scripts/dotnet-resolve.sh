#!/bin/bash

# Shared dotnet resolution logic for all scripts
# Source this file at the start of each script: source "$(dirname "$0")/dotnet-resolve.sh"

# Resolve dotnet from DOTNET_ROOT or PATH for portability
if [ -n "${DOTNET_ROOT:-}" ]; then
  DOTNET="${DOTNET_ROOT%/}/dotnet"
elif command -v dotnet &> /dev/null; then
  DOTNET="dotnet"
else
  # Common installation locations
  if [ -x "/usr/local/share/dotnet/dotnet" ]; then
    DOTNET="/usr/local/share/dotnet/dotnet"
  elif [ -x "$HOME/.dotnet/dotnet" ]; then
    DOTNET="$HOME/.dotnet/dotnet"
  else
    echo "❌ Error: Could not find 'dotnet' executable." >&2
    echo "   Please ensure .NET SDK is installed and either:" >&2
    echo "   - Add dotnet to your PATH, or" >&2
    echo "   - Set DOTNET_ROOT environment variable" >&2
    echo "" >&2
    echo "   Install .NET SDK: https://dotnet.microsoft.com/download" >&2
    exit 1
  fi
fi

# Verify dotnet is executable
if [ ! -x "$DOTNET" ]; then
  echo "❌ Error: Found dotnet at '$DOTNET' but it's not executable." >&2
  exit 1
fi

# Export for use in scripts
export DOTNET
