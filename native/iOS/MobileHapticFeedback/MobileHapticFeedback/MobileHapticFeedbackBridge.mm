#import <Foundation/Foundation.h>
#import "MobileHapticFeedback-Swift.h"

extern "C" {

    bool MHF_SupportsCoreHaptics(void) {
        return [MobileHapticFeedback supportsCoreHaptics];
    }

    void MHF_PrepareCoreHaptics(void) {
        [MobileHapticFeedback prepareCoreHaptics];
    }

    void MHF_StopCoreHaptics(void) {
        [MobileHapticFeedback stopCoreHaptics];
    }

    void MHF_PlayCoreImpact(float intensity, float sharpness, double durationSec) {
        [MobileHapticFeedback playCoreImpactWithIntensity:intensity sharpness:sharpness durationSec:durationSec];
    }

    void MHF_PlayCorePattern(const float* durationsSec, const float* amplitudes, int count) {
        if (!durationsSec || !amplitudes || count <= 0) return;

        NSMutableArray<NSNumber*>* ds = [NSMutableArray arrayWithCapacity:count];
        NSMutableArray<NSNumber*>* as = [NSMutableArray arrayWithCapacity:count];

        for (int i = 0; i < count; i++) {
            [ds addObject:@(durationsSec[i])];
            [as addObject:@(amplitudes[i])];
        }

        [MobileHapticFeedback playCorePatternWithDurationsSec:ds amplitudes:as];
    }

    void MHF_PlayUIKitImpact(int style) {
        [MobileHapticFeedback playUIKitImpactWithStyle:style];
    }

    void MHF_PlayUIKitSelection(void) {
        [MobileHapticFeedback playUIKitSelection];
    }

    void MHF_PlayUIKitNotification(int type) {
        [MobileHapticFeedback playUIKitNotificationWithType:type];
    }
}

