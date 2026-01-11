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
                        new float[] {
                            dit, symbolGap, dit, symbolGap,  dit, // S
                            letterGap,
                            dah, symbolGap, dah, symbolGap, dah, // O
                            letterGap,
                            dit, symbolGap, dit, symbolGap,  dit // S
                        },
                        new float[] {
                            1,0, 1,0, 1,
                            0,
                            1,0, 1,0, 1,
                            0,
                            1,0, 1,0, 1
                        }
                    );
                    break;
                case VibrationType.StepUp:
                    MobileHapticFeedback.PlayPattern(
                        new float[]
                        {
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f, 0.15f,
                            0.6f
                        },
                        new float[]
                        {
                            0.1f, 0f,
                            0.2f, 0f,
                            0.3f, 0f,
                            0.4f, 0f,
                            0.5f, 0f,
                            0.6f, 0f,
                            0.7f, 0f,
                            0.8f, 0f,
                            0.9f, 0f,
                            1.0f
                        }
                    );
                    break;
                case VibrationType.Heartbeat:
                    var lub = 0.07f;
                    var gap = 0.03f;
                    var dub = 0.10f;
                    var rest = 1.0f;
                    MobileHapticFeedback.PlayPattern(
                        new float[]
                        {
                            lub, gap, dub, rest,
                            lub, gap, dub, rest,
                            lub, gap, dub, rest,
                            lub, gap, dub, rest,
                            lub, gap, dub, rest,
                        },
                        new float[]
                        {
                            0.55f, 0f, 0.95f, 0f,
                            0.55f, 0f, 0.95f, 0f,
                            0.55f, 0f, 0.95f, 0f,
                            0.55f, 0f, 0.95f, 0f,
                            0.55f, 0f, 0.95f, 0f,
                        }
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