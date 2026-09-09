using UnityEngine;

namespace tsyk5.MobileHapticFeedback
{
    internal static class AndroidHapticFeedback
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string JavaClass = "com.tsyk5.mobilehapticfeedback.MobileHapticFeedback";

        private static AndroidJavaClass _pluginClass;
        private static AndroidJavaObject _activity;
        private static bool? _hasVibrator;
        private static bool? _supportsSharpness;

        private static AndroidJavaClass PluginClass => _pluginClass ??= new AndroidJavaClass(JavaClass);

        private static AndroidJavaObject Activity
        {
            get
            {
                if (_activity == null)
                {
                    using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _activity;
            }
        }

        public static bool HasVibrator()
        {
            return _hasVibrator ??= PluginClass.CallStatic<bool>("hasVibrator", Activity);
        }

        public static bool SupportsSharpness()
        {
            return _supportsSharpness ??= PluginClass.CallStatic<bool>("supportsSharpness", Activity);
        }

        public static void Stop()
        {
            PluginClass.CallStatic("stop", Activity);
        }

        public static void PlayImpact(float intensity, float sharpness, double durationSec)
        {
            PluginClass.CallStatic("playImpact", Activity, intensity, sharpness, durationSec);
        }

        public static void PlayPattern(float[] durationsSec, float[] amplitudes)
        {
            PluginClass.CallStatic("playPattern", Activity, durationsSec, amplitudes);
        }

        public static void PlaySelection()
        {
            PluginClass.CallStatic("playSelection", Activity);
        }

        public static void PlayNotification(NotificationType notificationType)
        {
            PluginClass.CallStatic("playNotification", Activity, (int)notificationType);
        }

        public static void PlayImpactStyle(ImpactStyle impactStyle)
        {
            PluginClass.CallStatic("playImpactStyle", Activity, (int)impactStyle);
        }
#endif
    }
}
