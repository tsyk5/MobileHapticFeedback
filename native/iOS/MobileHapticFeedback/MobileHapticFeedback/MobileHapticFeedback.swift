import Foundation
import CoreHaptics
import UIKit
import os.log

@objc public final class MobileHapticFeedback: NSObject {

    private static let log = OSLog(subsystem: "com.tsyk5.mobilehapticfeedback", category: "Haptics")

    // CoreHaptics
    private static var engine: CHHapticEngine?
    private static var lastPlayer: CHHapticPatternPlayer?
    
    private static var supportsHaptics: Bool {
        CHHapticEngine.capabilitiesForHardware().supportsHaptics
    }

    @objc public static func supportsCoreHaptics() -> Bool {
        supportsHaptics
    }
    
    // UIKit generators
    private static var impactGenerators: [UIImpactFeedbackGenerator.FeedbackStyle: UIImpactFeedbackGenerator] = [:]
    private static let selectionGenerator = UISelectionFeedbackGenerator()
    private static let notificationGenerator = UINotificationFeedbackGenerator()

    @objc public static func prepareCoreHaptics() {
        guard supportsHaptics else { return }

        // CoreHaptics engine
        if engine == nil {
            do {
                engine = try CHHapticEngine()
                engine?.stoppedHandler = { reason in
                    os_log("CHHapticEngine stopped: %{public}@", log: log, type: .info, "\(reason)")
                }
                engine?.resetHandler = {
                    do { try engine?.start() } catch { }
                }
            } catch {
                os_log("Failed to create CHHapticEngine: %{public}@", log: log, type: .error, "\(error)")
                engine = nil
                return
            }
        }

        do { try engine?.start() } catch {
            os_log("Failed to start CHHapticEngine: %{public}@", log: log, type: .error, "\(error)")
        }
        
        // UIKit generators
        selectionGenerator.prepare()
        notificationGenerator.prepare()
    }

    @objc public static func stopCoreHaptics() {
        do { try lastPlayer?.stop(atTime: 0) } catch { }
        lastPlayer = nil

        // NOTE: Cancel any pending vibrations via UIKit isn't needed
    }

    // CoreHaptics: Impact (single continuous event)
    @objc public static func playCoreImpact(intensity: Float, sharpness: Float, durationSec: Double) {
        guard supportsHaptics else { return }
        prepareCoreHaptics()

        let i = max(0, min(1, intensity))
        let s = max(0, min(1, sharpness))
        let d = max(0.01, durationSec)

        do {
            let ev = CHHapticEvent(
                eventType: .hapticContinuous,
                parameters: [
                    CHHapticEventParameter(parameterID: .hapticIntensity, value: i),
                    CHHapticEventParameter(parameterID: .hapticSharpness, value: s)
                ],
                relativeTime: 0,
                duration: d
            )

            let pat = try CHHapticPattern(events: [ev], parameters: [])
            let player = try engine?.makePlayer(with: pat)

            do { try lastPlayer?.stop(atTime: 0) } catch { }
            lastPlayer = player
            
            try player?.start(atTime: 0)
        } catch {
            os_log("playCoreImpact error: %{public}@", log: log, type: .error, "\(error)")
        }
    }

    // CoreHaptics: Pattern
    @objc public static func playCorePattern(durationsSec: [NSNumber], amplitudes: [NSNumber]) {
        guard supportsHaptics else { return }
        guard durationsSec.count == amplitudes.count, durationsSec.count > 0 else { return }
        prepareCoreHaptics()

        var t: TimeInterval = 0
        var events: [CHHapticEvent] = []
        events.reserveCapacity(durationsSec.count)

        for idx in 0..<durationsSec.count {
            let dur = max(0.01, durationsSec[idx].doubleValue)
            let amp = max(0, min(1, amplitudes[idx].floatValue))

            if amp > 0.0001 {
                let ev = CHHapticEvent(
                    eventType: .hapticContinuous,
                    parameters: [
                        CHHapticEventParameter(parameterID: .hapticIntensity, value: amp),
                        CHHapticEventParameter(parameterID: .hapticSharpness, value: 0.5)
                    ],
                    relativeTime: t,
                    duration: dur
                )
                events.append(ev)
            }
            t += dur
        }

        guard !events.isEmpty else { return }

        do {
            let pat = try CHHapticPattern(events: events, parameters: [])
            let player = try engine?.makePlayer(with: pat)
            
            do { try lastPlayer?.stop(atTime: 0) } catch { }
            lastPlayer = player

            try player?.start(atTime: 0)
        } catch {
            os_log("playCorePattern error: %{public}@", log: log, type: .error, "\(error)")
        }
    }

    // UIKit: Impact
    @objc public static func playUIKitImpact(style: Int) {
        let impactStyle: UIImpactFeedbackGenerator.FeedbackStyle

        switch style {
        case 0: impactStyle = .light
        case 1: impactStyle = .medium
        case 2: impactStyle = .heavy
        case 3: impactStyle = .soft
        case 4: impactStyle = .rigid
        default: impactStyle = .medium
        }

        // generator cache
        let gen: UIImpactFeedbackGenerator
        if let cached = impactGenerators[impactStyle] {
            gen = cached
        } else {
            if #available(iOS 17.0, *), let view = bestEffortView() {
                gen = UIImpactFeedbackGenerator(style: impactStyle, view: view)
            } else {
                gen = UIImpactFeedbackGenerator(style: impactStyle) // NOTE: deprecated
            }
            impactGenerators[impactStyle] = gen
        }

        gen.prepare()
        gen.impactOccurred()
    }
    
    private static func bestEffortView() -> UIView? {
        if #available(iOS 13.0, *) {
            let scenes = UIApplication.shared.connectedScenes
            let ws = scenes
                .compactMap { $0 as? UIWindowScene }
                .first(where: { $0.activationState == .foregroundActive })

            let window = ws?.windows.first(where: { $0.isKeyWindow }) ?? ws?.windows.first
            return window?.rootViewController?.view ?? window
        } else {
            let window = UIApplication.shared.windows.first(where: { $0.isKeyWindow }) ?? UIApplication.shared.windows.first
            return window?.rootViewController?.view ?? window
        }
    }

    // UIKit: Selection
    @objc public static func playUIKitSelection() {
        selectionGenerator.prepare()
        selectionGenerator.selectionChanged()
    }

    // UIKit: Notification
    @objc public static func playUIKitNotification(type: Int) {
        let t: UINotificationFeedbackGenerator.FeedbackType
        switch type {
        case 0: t = .success
        case 1: t = .warning
        case 2: t = .error
        default: t = .error
        }
        
        notificationGenerator.prepare()
        notificationGenerator.notificationOccurred(t)
    }
}
