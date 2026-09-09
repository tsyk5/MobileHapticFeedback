# Changelog

## 0.5.0 (unreleased)
- feat: Android 16+ envelope effects (`BasicEnvelopeBuilder`) for `PlayImpact` / `PlayPattern`, so `sharpness` is honored on supported devices
- feat: add `MobileHapticFeedback.IsSharpnessSupported`
- feat: `PatternSegment` gets an optional `sharpness` (default 0.5); iOS Core Haptics and Android 16+ envelope patterns use it per segment
- chore: Android compileSdk 34 -> 36
- fix: iOS framework deployment target 18.2 -> 15.0. The 18.2 build folded away the availability check
  around `UIImpactFeedbackGenerator(style:view:)` (iOS 17.5+), crashing `PlayImpact(ImpactStyle)` on iOS < 17.5
- fix: iOS UIKit generators are bound to the app's view via `init(view:)` on iOS 17.5+ (the view-less
  initializers are deprecated from the iOS 27 SDK)
- perf: iOS Core Haptics engine is created with `audioSession: nil` and `playsHapticsOnly`, and is
  restarted only when it actually stopped instead of on every play
- docs: minimum iOS is now 15 (the floor of Xcode 26 and Unity 6.3)
- fix: Android AAR now ships consumer ProGuard rules, so enabling Minify (R8) in Unity no longer strips the plugin class
- change: Android passes `VibrationAttributes` (API 33+). UIKit-like APIs use `USAGE_TOUCH` (follows "Touch feedback"),
  Core Haptics-like `PlayImpact` / `PlayPattern` use `USAGE_MEDIA` (follows "Media vibration"). Previously the system
  inferred the usage from each effect, so short and long effects followed different user settings
- fix: Android `PlayImpact` with intensity 0 is now silent (matches iOS) instead of vibrating at minimum amplitude
- fix: Android 10 (API 29) now uses predefined effects for selection / light / medium / heavy instead of a one-shot buzz

## 0.4.0
- fix: inverted waveform patterns on Android devices without amplitude control
- fix: JNI global reference leak on Android
- feat: add type-safe `PlayPattern(params PatternSegment[])` and deprecate the array version
- fix: raise haptic duration cap from 2s to 10s

## 0.3.0
- fix: Android haptics were silent on devices lacking predefined-effect / composition-primitive support

## 0.2.1
- fix: include dSYMs in xcframework for both iOS device and simulator

## 0.2.0
- fix: fix function arguments

## 0.1.1
- fix: samples import (v0.1.1)

## 0.1.0
- Initial public release
- iOS: Core Haptics support
- iOS: UIKit feedback support (Impact / Selection / Notification)
- Android: vibration-based haptics support
- Unity sample scene (Sample01)
