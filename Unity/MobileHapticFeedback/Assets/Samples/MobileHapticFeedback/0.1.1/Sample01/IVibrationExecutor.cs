using UnityEngine;

namespace tsyk5.MobileHapticFeedback.Sample01
{
    public interface IVibrationExecutor
    {
        void Vibrate(VibrationType type);
        void Vibrate(float intensity, float sharpness, float duration);
    }
    
    public class VibrationExecutor : IVibrationExecutor
    {
        void IVibrationExecutor.Vibrate(VibrationType type)
        {
            switch (type)
            {
                // Impact Style (UI Kit)
                case VibrationType.Light:
                    MobileHapticFeedback.PlayImpact(ImpactStyle.Light);
                    break;
                case VibrationType.Medium:
                    MobileHapticFeedback.PlayImpact(ImpactStyle.Medium);
                    break;
                case VibrationType.Heavy:
                    MobileHapticFeedback.PlayImpact(ImpactStyle.Heavy);
                    break;
                case VibrationType.Soft:
                    MobileHapticFeedback.PlayImpact(ImpactStyle.Soft);
                    break;
                case VibrationType.Rigid:
                    MobileHapticFeedback.PlayImpact(ImpactStyle.Rigid);
                    break;

                // Notification Style (UI Kit)
                case VibrationType.Success:
                    MobileHapticFeedback.PlayNotification(NotificationType.Success);
                    break;
                case VibrationType.Warning:
                    MobileHapticFeedback.PlayNotification(NotificationType.Warning);
                    break;
                case VibrationType.Error:
                    MobileHapticFeedback.PlayNotification(NotificationType.Error);
                    break;
                
                // Selection Style (UI Kit)
                case VibrationType.Selection:
                    MobileHapticFeedback.PlaySelection();
                    break;
                
                // Core Haptics (Patterns)
                case VibrationType.Sos: // ... --- ...
                    const float unit = 0.1f;
                    PatternSegment dit = new(unit, 1f);
                    PatternSegment dah = new(unit * 3, 1f);
                    PatternSegment symbolGap = new(unit, 0f);
                    PatternSegment letterGap = new(unit * 1.5f, 0f);
                    MobileHapticFeedback.PlayPattern(
                        dit, symbolGap, dit, symbolGap, dit, // S
                        letterGap,
                        dah, symbolGap, dah, symbolGap, dah, // O
                        letterGap,
                        dit, symbolGap, dit, symbolGap, dit  // S
                    );
                    break;
                case VibrationType.StepUp:
                    PatternSegment stepGap = new(0.15f, 0f);
                    MobileHapticFeedback.PlayPattern(
                        new(0.6f, 0.1f), stepGap,
                        new(0.6f, 0.2f), stepGap,
                        new(0.6f, 0.3f), stepGap,
                        new(0.6f, 0.4f), stepGap,
                        new(0.6f, 0.5f), stepGap,
                        new(0.6f, 0.6f), stepGap,
                        new(0.6f, 0.7f), stepGap,
                        new(0.6f, 0.8f), stepGap,
                        new(0.6f, 0.9f), stepGap,
                        new(0.6f, 1.0f)
                    );
                    break;
                case VibrationType.Heartbeat:
                    PatternSegment lub = new(0.07f, 0.55f);
                    PatternSegment gap = new(0.03f, 0f);
                    PatternSegment dub = new(0.10f, 0.95f);
                    PatternSegment rest = new(1.0f, 0f);
                    MobileHapticFeedback.PlayPattern(
                        lub, gap, dub, rest,
                        lub, gap, dub, rest,
                        lub, gap, dub, rest,
                        lub, gap, dub, rest,
                        lub, gap, dub, rest
                    );
                    break;
            }
        }

        void IVibrationExecutor.Vibrate(float intensity, float sharpness, float duration)
        {
            // Core Haptics (transient)
            MobileHapticFeedback.PlayImpact(intensity, sharpness, duration);
        }
    }
}