using System.Runtime.InteropServices;
using UnityEngine;

namespace tsyk5.MobileHapticFeedback
{
    internal static class IOSHapticFeedback
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern bool  MHF_SupportsCoreHaptics();
        [DllImport("__Internal")] private static extern void  MHF_PrepareCoreHaptics();
        [DllImport("__Internal")] private static extern void  MHF_StopCoreHaptics();

        [DllImport("__Internal")] private static extern void  MHF_PlayCoreImpact(float intensity, float sharpness, double durationSec);
        [DllImport("__Internal")] private static extern void  MHF_PlayCorePattern(float[] durationsSec, float[] amplitudes, int count);

        [DllImport("__Internal")] private static extern void  MHF_PlayUIKitImpact(int style);
        [DllImport("__Internal")] private static extern void  MHF_PlayUIKitSelection();
        [DllImport("__Internal")] private static extern void  MHF_PlayUIKitNotification(int type);
#else
        private static bool  MHF_SupportsCoreHaptics() => false;
        private static void  MHF_PrepareCoreHaptics() {}
        private static void  MHF_StopCoreHaptics() {}

        private static void  MHF_PlayCoreImpact(float a,float b,double c){}
        private static void  MHF_PlayCorePattern(float[] a,float[] b,int c){}

        private static void  MHF_PlayUIKitImpact(int style) {}
        private static void  MHF_PlayUIKitSelection() {}
        private static void  MHF_PlayUIKitNotification(int type) {}
#endif

        public static bool SupportsCoreHaptics => MHF_SupportsCoreHaptics();
        public static void PrepareCoreHaptics() => MHF_PrepareCoreHaptics();
        public static void StopCoreHaptics() => MHF_StopCoreHaptics();

        public static void PlayCoreImpact(float intensity, float sharpness, double durationSec)
            => MHF_PlayCoreImpact(intensity, sharpness, durationSec);

        public static void PlayCorePattern(float[] durationsSec, float[] amplitudes)
        {
            if (durationsSec == null || amplitudes == null) return;
            if (durationsSec.Length == 0 || durationsSec.Length != amplitudes.Length) return;

            MHF_PlayCorePattern(durationsSec, amplitudes, durationsSec.Length);
        }

        public static void PlayUIKitImpact(int style) => MHF_PlayUIKitImpact(style);
        public static void PlayUIKitSelection() => MHF_PlayUIKitSelection();
        public static void PlayUIKitNotification(int type) => MHF_PlayUIKitNotification(type);
    }
}

