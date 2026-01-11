# MobileHapticFeedback

Cross-platform haptic feedback library for Unity (iOS / Android).

## Features
- iOS: Core Haptics (CHHapticEngine)
- iOS: UIKit feedback (Impact / Selection / Notification)
- Android: VibrationEffect-based haptics (API 26+)
- Unified API for Unity

> Note (Android):
> UIKit-style feedback and Core Haptics are conceptually mapped to the closest available vibration patterns.

## Supported Platforms
- iOS 13+
- Android API 26+
- Unity 6000.0 or later

## Installation (Unity Package Manager)

Add via Git URL: 
```
https://github.com/tsyk5/MobileHapticFeedback.git?path=package/com.tsyk5.mobilehapticfeedback
```

To install a specific version, append a tag:
```
https://github.com/tsyk5/MobileHapticFeedback.git?path=package/com.tsyk5.mobilehapticfeedback#v0.1.1
```


## Quick Start

```csharp
using tsyk5.MobileHapticFeedback;

// UIKit-like APIs
MobileHapticFeedback.PlayImpact(ImpactStyle.Medium);
MobileHapticFeedback.PlaySelection();
MobileHapticFeedback.PlayNotification(NotificationType.Success);

// Core Haptics-like API 
// (iOS uses Core Haptics; Android uses nearest mapping)
MobileHapticFeedback.PlayImpact(intensity: 0.6f, sharpness: 0.3f, durationSec: 0.2);
```

## Notes / Limitations

If you only use UIKit-style feedback (PlayImpact(ImpactStyle), PlaySelection, PlayNotification), you can call them without checking support.
On unsupported devices/settings, they may simply do nothing.

On Android, UIKit-style feedback and Core Haptics are mapped to the closest available vibration behavior.

Apps cannot reliably detect whether vibration/haptics are enabled in the OS settings across all devices.
The recommended approach is to trigger haptics and accept that it may be muted by user/system settings.

## Samples

Samples/Sample01 (Unity Package Manager Samples)

The sample scenes are provided for testing and reference purposes and are not required for runtime usage.

## License

MIT
