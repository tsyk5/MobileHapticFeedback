# MobileHapticFeedback

> 📘 Read this document in other languages:  
> - [日本語 (README.ja.md)](README.ja.md)

**MobileHapticFeedback** is a cross-platform haptic feedback library for **Unity**,  
providing a unified API for **iOS (Core Haptics / UIKit)** and **Android (VibrationEffect)**.

It allows you to use UIKit-style feedback, parameterized one-shot impacts,
and waveform-based haptic patterns with a consistent programming model across platforms.

## Features
- **iOS**
  - Core Haptics–based impacts (CHHapticEngine)
  - UIKit-style feedback (Impact / Selection / Notification)
- **Android**
  - VibrationEffect-based haptics (API 26+)
- **Unity**
  - Unified cross-platform API
  - Safe no-op behavior when haptics are unavailable or muted

> **⚠️ Note (Android)**  
> UIKit-style feedback and Core Haptics–like APIs are conceptually mapped to  
> the closest available vibration patterns supported by the platform.

## Supported Platforms
- iOS 13+
- Android API 26+
- Unity 6000.0 or later

## Installation ( Unity Package Manager )

Add via Git URL: 
```
https://github.com/tsyk5/MobileHapticFeedback.git?path=package/com.tsyk5.mobilehapticfeedback
```

To install a specific version, append a tag:
```
https://github.com/tsyk5/MobileHapticFeedback.git?path=package/com.tsyk5.mobilehapticfeedback#v0.1.1
```


## Quick Start

### UIKit-like APIs

```csharp
MobileHapticFeedback.PlayImpact(ImpactStyle.Medium);
MobileHapticFeedback.PlaySelection();
MobileHapticFeedback.PlayNotification(NotificationType.Success);
```
These calls are safe to use without checking availability.

If haptics are unsupported or muted by OS/user settings, calls may simply do nothing.

### Core Haptics-like API ( One-shot impact )

```csharp
MobileHapticFeedback.Prepare();
MobileHapticFeedback.PlayImpact(intensity: 0.6f, sharpness: 0.3f, durationSec: 0.2);

MobileHapticFeedback.Stop();
```
Parameters
- `intensity` (0..1): Strength of the haptic.
- `sharpness` (0..1): Crispness / sharp edge of the haptic (iOS only).
- `durationSec` (sec): Duration of the haptic.

> <b>⚠️ Note (Android)</b>: <br/>
Android does not support the sharpness parameter.<br/>
Changing this value does not affect vibration behavior.

### Patterns API （ Waveform-style ）

```csharp
MobileHapticFeedback.Prepare();


var durationsSec = new float[]
    {
        0.6f, 0.15f
    };
var amplitudes = new float[]
    {
        0.1f, 0f
    }
MobileHapticFeedback.PlayPattern(durationsSec,amplitudes);
```
Parameters

- durations (sec): Duration of each segment.
- amplitudes (0..1): `0` means silence.`1` means maximum strength.

## Samples ( Sample01 )

The following patterns are available in the Sample01 scene.

After building the Sample01 scene, press the buttons in each section to
test and experience the haptic feedback behavior on a real device.

> ⚠️
<br/>Haptics cannot be reproduced in the Unity Editor.Please run the sample on a physical device.

### ⭐️ Core Haptics-like API ( One-shot impact )

<img src="images/unity-pattern-one-shot.png" width="520" />

This section demonstrates a single, parameterized haptic impact.

Adjust the sliders and press “Play Impact” to trigger the haptic.

Press “Stop” at any time to interrupt playback, even during the duration.

### ⭐️ Patterns API （ Waveform-style ）

<img src="images/unity-patterns.png" width="520" />

Construct custom haptic patterns by combining explicit
duration (time) and amplitude (strength) values.

All patterns can be interrupted at any time by pressing “Stop”.

### SOS

<img src="images/pattern-sos.png" width="520" />

Represents the Morse code SOS (… --- …) using haptics.

Short and long pulses are combined to form a clear emergency signal.

### Heartbeat

<img src="images/pattern-heartbeat.png" width="520" />

A rhythmic pattern inspired by a human heartbeat.

Double pulses and pauses create a natural, biological rhythm.

### StepUp

<img src="images/pattern-stepup.png" width="520" />

A pattern where haptic intensity gradually increases over time.

Useful for expressing buildup, progress, or emphasis.

### ⭐️ UIKit-like APIs

<img src="images/unity-uikit.png" width="520" />

UIKit-style haptics follow Apple’s system-defined haptic behaviors.

These APIs provide standardized feedback patterns for common UI interactions,
ensuring consistency across iOS applications.

For detailed design intent and official diagrams, please refer to Apple’s
Human Interface Guidelines:

<a href="https://developer.apple.com/design/human-interface-guidelines/playing-haptics">
Apple – Human Interface Guidelines / Playing Haptics
</a>


### Light
<img src="images/uikit-light.png" width="520" />

Indicates a collision between small or lightweight UI objects.

### Medium
<img src="images/uikit-medium.png" width="520" />

Indicates a collision between medium-sized or medium-weight UI objects.

### Heavy
<img src="images/uikit-heavy.png" width="520" />

Indicates a collision between large or heavyweight UI objects.

### Soft
<img src="images/uikit-soft.png" width="520" />

Indicates a collision between soft or flexible UI objects.

### Rigid
<img src="images/uikit-rigid.png" width="520" />

Indicates a collision between hard or inflexible UI objects.

### Success
<img src="images/uikit-success.png" width="520" />

Indicates that a task or action has completed.

### Warning
<img src="images/uikit-warning.png" width="520" />

Indicates that a task or action has produced a warning of some kind.

### Error
<img src="images/uikit-error.png" width="520" />

Indicates that an error has occurred.

### Selection
<img src="images/uikit-selection.png" width="520" />

Indicates that a UI element’s values are changing.

## License

MIT
