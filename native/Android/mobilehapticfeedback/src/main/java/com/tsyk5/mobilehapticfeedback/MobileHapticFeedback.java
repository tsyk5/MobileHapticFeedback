package com.tsyk5.mobilehapticfeedback;

import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.VibratorManager;

public final class MobileHapticFeedback {

    private MobileHapticFeedback() {}

    // ImpactStyle
    public static final int IMPACT_LIGHT  = 0;
    public static final int IMPACT_MEDIUM = 1;
    public static final int IMPACT_HEAVY  = 2;
    public static final int IMPACT_SOFT   = 3;
    public static final int IMPACT_RIGID  = 4;

    // NotificationType
    public static final int NOTIF_SUCCESS = 0;
    public static final int NOTIF_WARNING = 1;
    public static final int NOTIF_ERROR   = 2;

    private static Vibrator getVibrator(Context ctx) {
        if (ctx == null) return null;

        if (Build.VERSION.SDK_INT >= 31) {
            VibratorManager vm = (VibratorManager) ctx.getSystemService(Context.VIBRATOR_MANAGER_SERVICE);
            return vm != null ? vm.getDefaultVibrator() : null;
        } else {
            @SuppressWarnings("deprecation")
            Vibrator v = (Vibrator) ctx.getSystemService(Context.VIBRATOR_SERVICE);
            return v;
        }
    }

    public static boolean hasVibrator(Context ctx) {
        Vibrator v = getVibrator(ctx);
        return v != null && v.hasVibrator();
    }

    public static void stop(Context ctx) {
        Vibrator v = getVibrator(ctx);
        if (v == null) return;
        try { v.cancel(); } catch (Throwable ignored) {}
    }

    // TODO: sharpness is not supported
    public static void playImpact(Context ctx, float intensity, float sharpness, double durationSec) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        float i = clamp01(intensity);
        long durationMs = clampLong((long) (durationSec * 1000.0), 10L, 2000L);

        int amp = clampInt((int) (i * 255f), 1, 255);

        if (Build.VERSION.SDK_INT >= 26) {
            int amplitude = v.hasAmplitudeControl() ? amp : VibrationEffect.DEFAULT_AMPLITUDE;
            VibrationEffect effect = VibrationEffect.createOneShot(durationMs, amplitude);
            v.vibrate(effect);
        } else {
            @SuppressWarnings("deprecation")
            long ms = durationMs;
            v.vibrate(ms);
        }
    }

    public static void playPattern(Context ctx, float[] durationsSec, float[] amplitudes) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;
        if (durationsSec == null || amplitudes == null) return;
        if (durationsSec.length == 0 || durationsSec.length != amplitudes.length) return;

        long[] timings = new long[durationsSec.length];
        for (int i = 0; i < durationsSec.length; i++) {
            long ms = (long) (durationsSec[i] * 1000.0);
            timings[i] = clampLong(ms, 0L, 5000L);
        }

        int[] amps = new int[amplitudes.length];
        for (int i = 0; i < amplitudes.length; i++) {
            int a = (int) (clamp01(amplitudes[i]) * 255f);
            amps[i] = clampInt(a, 0, 255);
        }

        int noRepeat = -1;

        if (Build.VERSION.SDK_INT >= 26) {
            VibrationEffect effect = v.hasAmplitudeControl()
                    ? VibrationEffect.createWaveform(timings, amps, noRepeat)
                    : VibrationEffect.createWaveform(timings, noRepeat);
            v.vibrate(effect);
        } else {
            @SuppressWarnings("deprecation")
            long[] t = timings;
            v.vibrate(t, noRepeat);
        }
    }

    // Equivalent to UIKit Selection
    public static void playSelection(Context ctx) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        if (Build.VERSION.SDK_INT >= 29) {
            v.vibrate(VibrationEffect.createPredefined(VibrationEffect.EFFECT_TICK));
        } else {
            playImpact(ctx, 0.15f, 0.5f, 0.03);
        }
    }

    // Equivalent to UIKit Impact
    public static void playImpactStyle(Context ctx, int style) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        int s = clampInt(style, IMPACT_LIGHT, IMPACT_RIGID);

        if (Build.VERSION.SDK_INT >= 29) {
            if (s == IMPACT_LIGHT)  { v.vibrate(VibrationEffect.createPredefined(VibrationEffect.EFFECT_TICK)); return; }
            if (s == IMPACT_MEDIUM) { v.vibrate(VibrationEffect.createPredefined(VibrationEffect.EFFECT_CLICK)); return; }
            if (s == IMPACT_HEAVY)  { v.vibrate(VibrationEffect.createPredefined(VibrationEffect.EFFECT_HEAVY_CLICK)); return; }
        }

        switch (s) {
            case IMPACT_LIGHT:  playImpact(ctx, 0.25f, 0.85f, 0.03);  break;
            case IMPACT_MEDIUM: playImpact(ctx, 0.55f, 0.85f, 0.04);  break;
            case IMPACT_HEAVY:  playImpact(ctx, 0.80f, 0.90f, 0.05);  break;
            case IMPACT_SOFT:   playImpact(ctx, 0.30f, 0.20f, 0.035); break;
            case IMPACT_RIGID:  playImpact(ctx, 0.80f, 1.00f, 0.02);  break;
            default:            playImpact(ctx, 0.45f, 0.85f, 0.04);  break;
        }
    }

    // Equivalent to UIKit Notification
    public static void playNotification(Context ctx, int type) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        int nt = clampInt(type, NOTIF_SUCCESS, NOTIF_ERROR);
        int noRepeat = -1;

        if (Build.VERSION.SDK_INT >= 30) {
            VibrationEffect.Composition comp = VibrationEffect.startComposition();
            if (nt == NOTIF_SUCCESS) {
                comp.addPrimitive(VibrationEffect.Composition.PRIMITIVE_TICK, 1.0f)
                        .addPrimitive(VibrationEffect.Composition.PRIMITIVE_CLICK, 1.0f, 240);
            } else if (nt == NOTIF_WARNING) {
                comp.addPrimitive(VibrationEffect.Composition.PRIMITIVE_CLICK, 1.0f)
                        .addPrimitive(VibrationEffect.Composition.PRIMITIVE_TICK, 1.0f, 240);
            } else {
                comp.addPrimitive(VibrationEffect.Composition.PRIMITIVE_TICK, 1.0f)
                        .addPrimitive(VibrationEffect.Composition.PRIMITIVE_TICK, 1.0f, 120)
                        .addPrimitive(VibrationEffect.Composition.PRIMITIVE_CLICK, 1.0f, 120)
                        .addPrimitive(VibrationEffect.Composition.PRIMITIVE_TICK, 1.0f, 120);
            }
            v.vibrate(comp.compose());
            return;
        }

        if (Build.VERSION.SDK_INT >= 26) {
            long[] timings;
            int[] amps;

            if (nt == NOTIF_SUCCESS) {
                timings = new long[]{55, 55, 53};
                amps = new int[]{178, 0, 255};
            } else if (nt == NOTIF_WARNING) {
                timings = new long[]{55, 70, 55};
                amps = new int[]{229, 0, 178};
            } else {
                timings = new long[]{51, 32, 55, 32, 55};
                amps = new int[]{204, 0, 204, 0, 255};
            }

            VibrationEffect effect = v.hasAmplitudeControl()
                    ? VibrationEffect.createWaveform(timings, amps, noRepeat)
                    : VibrationEffect.createWaveform(timings, noRepeat);
            v.vibrate(effect);
            return;
        }

        long[] timings;
        if (nt == NOTIF_SUCCESS) timings = new long[]{55, 55, 53};
        else if (nt == NOTIF_WARNING) timings = new long[]{55, 70, 55};
        else timings = new long[]{51, 32, 55, 32, 55};

        @SuppressWarnings("deprecation")
        long[] t = timings;
        v.vibrate(t, noRepeat);
    }

    private static float clamp01(float v) {
        return v < 0f ? 0f : (v > 1f ? 1f : v);
    }

    private static int clampInt(int v, int min, int max) {
        return Math.max(min, Math.min(max, v));
    }

    private static long clampLong(long v, long min, long max) {
        return Math.max(min, Math.min(max, v));
    }
}

