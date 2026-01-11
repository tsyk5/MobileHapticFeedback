#!/bin/bash
set -euo pipefail

PROJECT_PATH="../iOS/MobileHapticFeedback/MobileHapticFeedback.xcodeproj"
SCHEME="MobileHapticFeedback"
OUTPUT_DIR="iOS/build"

ARCHIVE_IOS_PATH="${OUTPUT_DIR}/MobileHapticFeedback_iOS.xcarchive"
ARCHIVE_SIM_PATH="${OUTPUT_DIR}/MobileHapticFeedback_iOS_Simulator.xcarchive"
XCFRAMEWORK_PATH="${OUTPUT_DIR}/MobileHapticFeedback.xcframework"

rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

echo "📦 Archiving for iOS device..."
xcodebuild archive \
  -project "$PROJECT_PATH" \
  -scheme "$SCHEME" \
  -configuration Release \
  -destination "generic/platform=iOS" \
  -archivePath "$ARCHIVE_IOS_PATH" \
  SKIP_INSTALL=NO \
  BUILD_LIBRARY_FOR_DISTRIBUTION=YES

echo "📦 Archiving for iOS Simulator..."
xcodebuild archive \
  -project "$PROJECT_PATH" \
  -scheme "$SCHEME" \
  -configuration Release \
  -destination "generic/platform=iOS Simulator" \
  -archivePath "$ARCHIVE_SIM_PATH" \
  SKIP_INSTALL=NO \
  BUILD_LIBRARY_FOR_DISTRIBUTION=YES

echo "🧩 Creating XCFramework..."
rm -rf "$XCFRAMEWORK_PATH"
xcodebuild -create-xcframework \
  -framework "${ARCHIVE_IOS_PATH}/Products/Library/Frameworks/${SCHEME}.framework" \
  -framework "${ARCHIVE_SIM_PATH}/Products/Library/Frameworks/${SCHEME}.framework" \
  -output "$XCFRAMEWORK_PATH"

echo "✅ Done: $XCFRAMEWORK_PATH"
