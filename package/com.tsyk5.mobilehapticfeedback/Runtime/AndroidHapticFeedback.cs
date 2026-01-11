using UnityEngine;

namespace tsyk5.MobileHapticFeedback
{
    internal static class AndroidHapticFeedback
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string JavaClass = "com.tsyk5.mobilehapticfeedback.MobileHapticFeedback";
    
        private static AndroidJavaObject Activity
        {
            get
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    
        public static bool HasVibrator()
        {
            using var jc = new AndroidJavaClass(JavaClass);
            return jc.CallStatic<bool>("hasVibrator", Activity);
        }
    
        public static void Stop()
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("stop", Activity);
        }
    
        public static void PlayImpact(float intensity, float sharpness, double durationSec)
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("playImpact", Activity, intensity, sharpness, durationSec);
        }
    
        public static void PlayPattern(float[] durationsSec, float[] amplitudes)
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("playPattern", Activity, durationsSec, amplitudes);
        }
    
        public static void PlaySelection()
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("playSelection", Activity);
        }
    
        public static void PlayNotification(NotificationType notificationType)
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("playNotification", Activity, (int)notificationType);
        }
    
        public static void PlayImpactStyle(ImpactStyle impactStyle)
        {
            using var jc = new AndroidJavaClass(JavaClass);
            jc.CallStatic("playImpactStyle", Activity, (int)impactStyle);
        }
#endif
    }
}