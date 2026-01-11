using System;
using UnityEngine;

namespace tsyk5.MobileHapticFeedback
{
    public static class MobileHapticFeedback
    {
        public static bool IsSupported
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
        return IOSHapticFeedback.SupportsCoreHaptics;
#elif UNITY_ANDROID && !UNITY_EDITOR
        return AndroidHapticFeedback.HasVibrator();
#else
                return false;
#endif
            }
        }

        public static void Prepare()
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PrepareCoreHaptics();
#endif
        }

        public static void Stop()
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.StopCoreHaptics();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.Stop();
#endif
        }

        public static void PlayImpact(float intensity, float sharpness, double durationSec)
        {
            intensity = Mathf.Clamp01(intensity);
            sharpness = Mathf.Clamp01(sharpness);
            durationSec = Math.Clamp(durationSec, 0.01, 2.0);

#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayCoreImpact(intensity, sharpness, durationSec);
#elif UNITY_ANDROID && !UNITY_EDITOR
            // TODO: sharpness is not supported
            AndroidHapticFeedback.PlayImpact(intensity, sharpness, durationSec);
#else
            Debug.Log($"[Editor] PlayImpact({intensity:F2}, {sharpness:F2}, {durationSec:F2}s)");
#endif
        }

        public static void PlayPattern(float[] durationsSec, float[] amplitudes)
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayCorePattern(durationsSec, amplitudes);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayPattern(durationsSec, amplitudes);
#else
            Debug.Log("[Editor] PlayPattern");
#endif
        }

        public static void PlayImpact(ImpactStyle impactStyle)
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayUIKitImpact((int)impactStyle);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayImpactStyle((int)impactStyle);
#else
            Debug.Log($"[Editor] UIKitImpact({impactStyle})");
#endif
        }

        public static void PlaySelection()
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayUIKitSelection();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlaySelection();
#else
            Debug.Log("[Editor] UIKitSelection");
#endif
        }

        public static void PlayNotification(NotificationType notificationType)
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayUIKitNotification((int)notificationType);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayNotification((int)notificationType);
#else
            Debug.Log($"[Editor] UIKitNotification({notificationType})");
#endif
        }
    }
}