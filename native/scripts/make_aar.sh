#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

ANDROID_DIR="$REPO_ROOT/native/Android"
MODULE="mobilehapticfeedback"

AAR_PATH="$ANDROID_DIR/$MODULE/build/outputs/aar/$MODULE-release.aar"

OUT_DIR="$SCRIPT_DIR/android/build"
OUT_AAR_PATH="$OUT_DIR/$MODULE-release.aar"

echo "📦 Building Android AAR..."
cd "$ANDROID_DIR"
./gradlew :$MODULE:clean :$MODULE:assembleRelease

echo "📦 Copying AAR into scripts build dir..."
mkdir -p "$OUT_DIR"
cp -f "$AAR_PATH" "$OUT_AAR_PATH"

echo "✅ Done!"
echo "AAR: $OUT_AAR_PATH"

if command -v open >/dev/null 2>&1; then
  open "$OUT_DIR"
fi

