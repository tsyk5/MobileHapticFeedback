# Changelog

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
