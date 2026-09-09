package com.tsyk5.mobilehapticfeedback;

import android.annotation.TargetApi;
import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.VibratorManager;
import android.os.vibrator.VibratorEnvelopeEffectInfo;

import java.util.Arrays;

public final class MobileHapticFeedback {

    private MobileHapticFeedback() {}

    // Keep in sync with MobileHapticFeedback.MinDurationSec / MaxDurationSec (C#)
    private static final long MIN_DURATION_MS = 10L;
    private static final long MAX_DURATION_MS = 10_000L;

    // Keep in sync with the fixed sharpness used by playCorePattern (iOS)
    private static final float PATTERN_SHARPNESS = 0.5f;

    // Guaranteed minimum for devices that support envelope effects (API 36+)
    private static final long DEFAULT_MIN_CONTROL_POINT_MS = 20L;

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

    /**
     * True when the device can render the sharpness parameter
     * (Android 16+ envelope effects, see VibrationEffect.BasicEnvelopeBuilder).
     */
    public static boolean supportsSharpness(Context ctx) {
        Vibrator v = getVibrator(ctx);
        return v != null && v.hasVibrator() && supportsEnvelopeEffects(v);
    }

    public static void stop(Context ctx) {
        Vibrator v = getVibrator(ctx);
        if (v == null) return;
        try { v.cancel(); } catch (Throwable ignored) {}
    }

    // NOTE: sharpness is only honored on devices that support envelope effects (API 36+)
    public static void playImpact(Context ctx, float intensity, float sharpness, double durationSec) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        float i = clamp01(intensity);
        float s = clamp01(sharpness);
        long durationMs = clampLong((long) (durationSec * 1000.0), MIN_DURATION_MS, MAX_DURATION_MS);

        if (supportsEnvelopeEffects(v)) {
            playEnvelopeImpact(v, i, s, durationMs);
            return;
        }

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
            timings[i] = clampLong(ms, 0L, MAX_DURATION_MS);
        }

        float[] amps01 = new float[amplitudes.length];
        for (int i = 0; i < amplitudes.length; i++) {
            amps01[i] = clamp01(amplitudes[i]);
        }

        if (supportsEnvelopeEffects(v)) {
            playEnvelopePattern(v, timings, amps01, PATTERN_SHARPNESS);
            return;
        }

        int[] amps = new int[amps01.length];
        for (int i = 0; i < amps01.length; i++) {
            amps[i] = clampInt((int) (amps01[i] * 255f), 0, 255);
        }

        vibrateWaveform(v, timings, amps);
    }

    // Equivalent to UIKit Selection
    public static void playSelection(Context ctx) {
        Vibrator v = getVibrator(ctx);
        if (v == null || !v.hasVibrator()) return;

        if (isEffectSupported(v, VibrationEffect.EFFECT_TICK)) {
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
            int effectId = -1;
            if (s == IMPACT_LIGHT)  effectId = VibrationEffect.EFFECT_TICK;
            else if (s == IMPACT_MEDIUM) effectId = VibrationEffect.EFFECT_CLICK;
            else if (s == IMPACT_HEAVY)  effectId = VibrationEffect.EFFECT_HEAVY_CLICK;

            if (effectId != -1 && isEffectSupported(v, effectId)) {
                v.vibrate(VibrationEffect.createPredefined(effectId));
                return;
            }
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

        if (Build.VERSION.SDK_INT >= 30
                && areAllPrimitivesSupported(v,
                        VibrationEffect.Composition.PRIMITIVE_TICK,
                        VibrationEffect.Composition.PRIMITIVE_CLICK)) {
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

        vibrateWaveform(v, timings, amps);
    }

    // ---- Envelope effects (API 36+) ----------------------------------------------------------

    private static boolean supportsEnvelopeEffects(Vibrator v) {
        if (Build.VERSION.SDK_INT < 36) return false;
        try {
            return v.areEnvelopeEffectsSupported();
        } catch (Throwable ignored) {
            return false;
        }
    }

    @TargetApi(36)
    private static long minControlPointMs(Vibrator v) {
        long min = DEFAULT_MIN_CONTROL_POINT_MS;
        try {
            VibratorEnvelopeEffectInfo info = v.getEnvelopeEffectInfo();
            if (info != null) min = info.getMinControlPointDurationMillis();
        } catch (Throwable ignored) {}
        return Math.max(1L, min);
    }

    // Single continuous event: ramp up as fast as the hardware allows, hold, ramp down.
    // Impacts shorter than two control points are stretched to that minimum (typically 40ms).
    @TargetApi(36)
    private static void playEnvelopeImpact(Vibrator v, float intensity, float sharpness, long durationMs) {
        long min = minControlPointMs(v);
        long hold = durationMs - 2 * min;

        VibrationEffect.BasicEnvelopeBuilder b = new VibrationEffect.BasicEnvelopeBuilder()
                .setInitialSharpness(sharpness)
                .addControlPoint(intensity, sharpness, min);
        if (hold > 0) {
            b.addControlPoint(intensity, sharpness, hold);
        }
        b.addControlPoint(0f, sharpness, min);

        v.vibrate(b.build());
    }

    // Each segment becomes "transition to the target amplitude as fast as possible, then hold".
    // The transition is taken out of the segment's own duration so the overall timing is preserved.
    // Segments shorter than the minimum control-point duration are stretched to it.
    @TargetApi(36)
    private static void playEnvelopePattern(Vibrator v, long[] timings, float[] amps01, float sharpness) {
        long min = minControlPointMs(v);

        VibrationEffect.BasicEnvelopeBuilder b = new VibrationEffect.BasicEnvelopeBuilder()
                .setInitialSharpness(sharpness);

        int points = 0;
        float lastIntensity = 0f;

        for (int i = 0; i < timings.length; i++) {
            long t = timings[i];
            float a = amps01[i];
            if (t <= 0) continue;

            if (a > 0f) {
                b.addControlPoint(a, sharpness, min);
                points++;
                if (t > min) {
                    b.addControlPoint(a, sharpness, t - min);
                    points++;
                }
            } else {
                b.addControlPoint(0f, sharpness, Math.max(t, min));
                points++;
            }
            lastIntensity = a;
        }

        if (points == 0) return;

        // Envelope effects must end at zero intensity
        if (lastIntensity > 0f) {
            b.addControlPoint(0f, sharpness, min);
        }

        v.vibrate(b.build());
    }

    // ---- Waveform fallback -------------------------------------------------------------------

    private static void vibrateWaveform(Vibrator v, long[] timings, int[] amps) {
        int noRepeat = -1;

        if (Build.VERSION.SDK_INT >= 26 && v.hasAmplitudeControl()) {
            v.vibrate(VibrationEffect.createWaveform(timings, amps, noRepeat));
            return;
        }

        long[] onOff = toOnOffTimings(timings, amps);

        if (Build.VERSION.SDK_INT >= 26) {
            v.vibrate(VibrationEffect.createWaveform(onOff, noRepeat));
        } else {
            @SuppressWarnings("deprecation")
            long[] t = onOff;
            v.vibrate(t, noRepeat);
        }
    }

    // NOTE: amplitude-less waveform APIs read timings as alternating off/on *starting with off*,
    //       so timing/amplitude pairs must be folded (amp > 0 = on, adjacent same-state merged).
    private static long[] toOnOffTimings(long[] timings, int[] amps) {
        long[] out = new long[timings.length + 1]; // +1: leading off period

        int length = 1;
        boolean isOn = false;

        for (int i = 0; i < timings.length; i++) {
            boolean segmentIsOn = amps[i] > 0;
            if (segmentIsOn == isOn) {
                out[length - 1] += timings[i];
            } else {
                out[length++] = timings[i];
                isOn = segmentIsOn;
            }
        }

        return length == out.length ? out : Arrays.copyOf(out, length);
    }

    private static boolean isEffectSupported(Vibrator v, int effectId) {
        if (Build.VERSION.SDK_INT < 30) return false;
        try {
            int[] support = v.areEffectsSupported(effectId);
            return support.length > 0
                    && support[0] == Vibrator.VIBRATION_EFFECT_SUPPORT_YES;
        } catch (Throwable ignored) {
            return false;
        }
    }

    private static boolean areAllPrimitivesSupported(Vibrator v, int... primitives) {
        if (Build.VERSION.SDK_INT < 30) return false;
        try {
            return v.areAllPrimitivesSupported(primitives);
        } catch (Throwable ignored) {
            return false;
        }
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
