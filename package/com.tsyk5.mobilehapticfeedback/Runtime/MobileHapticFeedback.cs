using System;
using UnityEngine;

namespace tsyk5.MobileHapticFeedback
{
    public static class MobileHapticFeedback
    {
        public const double MinDurationSec = 0.01;
        public const double MaxDurationSec = 10.0;

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

        /// <summary>
        /// True when the sharpness parameter of <see cref="PlayImpact(float, float, double)"/> is
        /// honored by the device. iOS: Core Haptics. Android: envelope effects (Android 16+ on
        /// supported hardware). When false, sharpness is silently ignored.
        /// </summary>
        public static bool IsSharpnessSupported
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
                return IOSHapticFeedback.SupportsCoreHaptics;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return AndroidHapticFeedback.SupportsSharpness();
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

            durationSec = Math.Clamp(durationSec, MinDurationSec, MaxDurationSec);

#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayCoreImpact(intensity, sharpness, durationSec);
#elif UNITY_ANDROID && !UNITY_EDITOR
            // NOTE: sharpness is honored only on devices with envelope-effect support (see IsSharpnessSupported)
            AndroidHapticFeedback.PlayImpact(intensity, sharpness, durationSec);
#else
            Debug.Log($"[Editor] PlayImpact({intensity:F2}, {sharpness:F2}, {durationSec:F2}s)");
#endif
        }

        public static void PlayPattern(params PatternSegment[] pattern)
        {
            if (pattern == null || pattern.Length == 0) return;

            var durationsSec = new float[pattern.Length];
            var amplitudes = new float[pattern.Length];
            var sharpnesses = new float[pattern.Length];
            for (int i = 0; i < pattern.Length; i++)
            {
                durationsSec[i] = pattern[i].DurationSec;
                amplitudes[i] = pattern[i].Amplitude;
                sharpnesses[i] = pattern[i].Sharpness;
            }

            PlayPatternCore(durationsSec, amplitudes, sharpnesses);
        }

        [Obsolete("Parallel arrays allow length mismatches that fail silently. Use PlayPattern(params PatternSegment[]) instead.")]
        public static void PlayPattern(float[] durationsSec, float[] amplitudes)
        {
            if (durationsSec == null || amplitudes == null) return;
            if (durationsSec.Length == 0 || durationsSec.Length != amplitudes.Length) return;

            var sharpnesses = new float[durationsSec.Length];
            for (int i = 0; i < sharpnesses.Length; i++)
                sharpnesses[i] = PatternSegment.DefaultSharpness;

            PlayPatternCore(durationsSec, amplitudes, sharpnesses);
        }

        private static void PlayPatternCore(float[] durationsSec, float[] amplitudes, float[] sharpnesses)
        {
            for (int i = 0; i < amplitudes.Length; i++)
            {
                amplitudes[i] = Mathf.Clamp01(amplitudes[i]);
                sharpnesses[i] = Mathf.Clamp01(sharpnesses[i]);
            }

#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayCorePattern(durationsSec, amplitudes, sharpnesses);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayPattern(durationsSec, amplitudes, sharpnesses);
#else
            Debug.Log("[Editor] PlayPattern");
#endif
        }

        public static void PlayImpact(ImpactStyle impactStyle)
        {
#if UNITY_IOS && !UNITY_EDITOR
            IOSHapticFeedback.PlayUIKitImpact(impactStyle);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayImpactStyle(impactStyle);
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
            IOSHapticFeedback.PlayUIKitNotification(notificationType);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHapticFeedback.PlayNotification(notificationType);
#else
            Debug.Log($"[Editor] UIKitNotification({notificationType})");
#endif
        }
    }
}