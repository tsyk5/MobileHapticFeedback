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
                    var dit = 0.1f;
                    var dah = dit * 3;
                    var symbolGap = dit;
                    var letterGap = dit * 1.5f;
                    MobileHapticFeedback.PlayPattern(
                        new(dit, 1f), new(symbolGap, 0f), new(dit, 1f), new(symbolGap, 0f), new(dit, 1f), // S
                        new(letterGap, 0f),
                        new(dah, 1f), new(symbolGap, 0f), new(dah, 1f), new(symbolGap, 0f), new(dah, 1f), // O
                        new(letterGap, 0f),
                        new(dit, 1f), new(symbolGap, 0f), new(dit, 1f), new(symbolGap, 0f), new(dit, 1f)  // S
                    );
                    break;
                case VibrationType.StepUp:
                    MobileHapticFeedback.PlayPattern(
                        new(0.6f, 0.1f), new(0.15f, 0f),
                        new(0.6f, 0.2f), new(0.15f, 0f),
                        new(0.6f, 0.3f), new(0.15f, 0f),
                        new(0.6f, 0.4f), new(0.15f, 0f),
                        new(0.6f, 0.5f), new(0.15f, 0f),
                        new(0.6f, 0.6f), new(0.15f, 0f),
                        new(0.6f, 0.7f), new(0.15f, 0f),
                        new(0.6f, 0.8f), new(0.15f, 0f),
                        new(0.6f, 0.9f), new(0.15f, 0f),
                        new(0.6f, 1.0f)
                    );
                    break;
                case VibrationType.Heartbeat:
                    var lub = 0.07f;
                    var gap = 0.03f;
                    var dub = 0.10f;
                    var rest = 1.0f;
                    MobileHapticFeedback.PlayPattern(
                        new(lub, 0.55f), new(gap, 0f), new(dub, 0.95f), new(rest, 0f),
                        new(lub, 0.55f), new(gap, 0f), new(dub, 0.95f), new(rest, 0f),
                        new(lub, 0.55f), new(gap, 0f), new(dub, 0.95f), new(rest, 0f),
                        new(lub, 0.55f), new(gap, 0f), new(dub, 0.95f), new(rest, 0f),
                        new(lub, 0.55f), new(gap, 0f), new(dub, 0.95f), new(rest, 0f)
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