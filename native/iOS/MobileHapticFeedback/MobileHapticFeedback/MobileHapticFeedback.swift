import Foundation
import CoreHaptics
import AVFoundation
import UIKit
import os.log

@objc public final class MobileHapticFeedback: NSObject {

    private static let log = OSLog(subsystem: "com.tsyk5.mobilehapticfeedback", category: "Haptics")

    // MARK: - Core Haptics state

    private static var engine: CHHapticEngine?
    private static var lastPlayer: CHHapticPatternPlayer?

    // The engine's stopped/reset handlers run on an internal queue, so the running flag is locked.
    private static let stateLock = NSLock()
    private static var _engineRunning = false
    private static var engineRunning: Bool {
        get { stateLock.lock(); defer { stateLock.unlock() }; return _engineRunning }
        set { stateLock.lock(); _engineRunning = newValue; stateLock.unlock() }
    }

    private static var supportsHaptics: Bool {
        CHHapticEngine.capabilitiesForHardware().supportsHaptics
    }

    @objc public static func supportsCoreHaptics() -> Bool {
        supportsHaptics
    }

    // MARK: - UIKit generators (created lazily so they can be bound to the app's view on iOS 17.5+)

    private static var impactGenerators: [UIImpactFeedbackGenerator.FeedbackStyle: UIImpactFeedbackGenerator] = [:]
    private static var selectionGenerator: UISelectionFeedbackGenerator?
    private static var notificationGenerator: UINotificationFeedbackGenerator?

    // MARK: - Engine lifecycle

    @objc public static func prepareCoreHaptics() {
        guard supportsHaptics else { return }

        ensureEngineRunning()

        selection().prepare()
        notification().prepare()
    }

    @objc public static func stopCoreHaptics() {
        do { try lastPlayer?.stop(atTime: 0) } catch { }
        lastPlayer = nil

        // NOTE: Cancel any pending vibrations via UIKit isn't needed
    }

    private static func createEngineIfNeeded() -> CHHapticEngine? {
        if let engine { return engine }

        do {
            // audioSession: nil -> the engine never touches the app's audio session (Unity owns it).
            // playsHapticsOnly -> audio events are ignored and start latency is reduced.
            let e = try CHHapticEngine(audioSession: nil)
            e.playsHapticsOnly = true

            e.stoppedHandler = { reason in
                os_log("CHHapticEngine stopped: %{public}@", log: log, type: .info, "\(reason.rawValue)")
                // Restart lazily on the next play (e.g. after .applicationSuspended / .audioSessionInterrupt)
                engineRunning = false
            }
            e.resetHandler = {
                os_log("CHHapticEngine reset", log: log, type: .info)
                engineRunning = false
                lastPlayer = nil
                // Media server reset: restart eagerly so the next play stays low-latency
                ensureEngineRunning()
            }

            engine = e
            return e
        } catch {
            os_log("Failed to create CHHapticEngine: %{public}@", log: log, type: .error, "\(error)")
            engine = nil
            return nil
        }
    }

    @discardableResult
    private static func ensureEngineRunning() -> CHHapticEngine? {
        guard let e = createEngineIfNeeded() else { return nil }
        if engineRunning { return e }

        do {
            try e.start()
            engineRunning = true
            return e
        } catch {
            os_log("Failed to start CHHapticEngine: %{public}@", log: log, type: .error, "\(error)")
            engineRunning = false
            return nil
        }
    }

    // Stops the previous player and starts a new one for `pattern`.
    // If the engine was stopped between the running-check and start, retry once after restarting.
    private static func play(pattern: CHHapticPattern) {
        do { try lastPlayer?.stop(atTime: 0) } catch { }
        lastPlayer = nil

        for attempt in 0..<2 {
            guard let e = ensureEngineRunning() else { return }
            do {
                let player = try e.makePlayer(with: pattern)
                try player.start(atTime: 0)
                lastPlayer = player
                return
            } catch {
                engineRunning = false
                if attempt == 1 {
                    os_log("Failed to play haptic pattern: %{public}@", log: log, type: .error, "\(error)")
                }
            }
        }
    }

    // MARK: - Core Haptics: Impact (single continuous event)

    @objc public static func playCoreImpact(intensity: Float, sharpness: Float, durationSec: Double) {
        guard supportsHaptics else { return }

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
            play(pattern: pat)
        } catch {
            os_log("playCoreImpact error: %{public}@", log: log, type: .error, "\(error)")
        }
    }

    // MARK: - Core Haptics: Pattern

    @objc public static func playCorePattern(durationsSec: [NSNumber], amplitudes: [NSNumber], sharpnesses: [NSNumber]) {
        guard supportsHaptics else { return }
        guard durationsSec.count > 0,
              durationsSec.count == amplitudes.count,
              durationsSec.count == sharpnesses.count else { return }

        var t: TimeInterval = 0
        var events: [CHHapticEvent] = []
        events.reserveCapacity(durationsSec.count)

        for idx in 0..<durationsSec.count {
            let dur = max(0.01, durationsSec[idx].doubleValue)
            let amp = max(0, min(1, amplitudes[idx].floatValue))
            let sharp = max(0, min(1, sharpnesses[idx].floatValue))

            if amp > 0.0001 {
                let ev = CHHapticEvent(
                    eventType: .hapticContinuous,
                    parameters: [
                        CHHapticEventParameter(parameterID: .hapticIntensity, value: amp),
                        CHHapticEventParameter(parameterID: .hapticSharpness, value: sharp)
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
            play(pattern: pat)
        } catch {
            os_log("playCorePattern error: %{public}@", log: log, type: .error, "\(error)")
        }
    }

    // MARK: - UIKit: Impact

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

        let gen = impact(impactStyle)
        gen.prepare()
        gen.impactOccurred()
    }

    // MARK: - UIKit: Selection

    @objc public static func playUIKitSelection() {
        let gen = selection()
        gen.prepare()
        gen.selectionChanged()
    }

    // MARK: - UIKit: Notification

    @objc public static func playUIKitNotification(type: Int) {
        let t: UINotificationFeedbackGenerator.FeedbackType
        switch type {
        case 0: t = .success
        case 1: t = .warning
        case 2: t = .error
        default: t = .error
        }

        let gen = notification()
        gen.prepare()
        gen.notificationOccurred(t)
    }

    // MARK: - UIKit generator factories
    //
    // iOS 17.5+ wants generators bound to a view (`init(view:)`); the view-less initializers are
    // deprecated from the iOS 27 SDK but remain the only option on iOS 15 - 17.4.
    // A generator is cached only once it could be bound to a view, so an early call before the
    // window exists does not pin a view-less generator forever.

    private static func impact(_ style: UIImpactFeedbackGenerator.FeedbackStyle) -> UIImpactFeedbackGenerator {
        if let cached = impactGenerators[style] { return cached }

        if #available(iOS 17.5, *) {
            if let view = bestEffortView() {
                let gen = UIImpactFeedbackGenerator(style: style, view: view)
                impactGenerators[style] = gen
                return gen
            }
            return UIImpactFeedbackGenerator(style: style)
        }

        let gen = UIImpactFeedbackGenerator(style: style)
        impactGenerators[style] = gen
        return gen
    }

    private static func selection() -> UISelectionFeedbackGenerator {
        if let cached = selectionGenerator { return cached }

        if #available(iOS 17.5, *) {
            if let view = bestEffortView() {
                let gen = UISelectionFeedbackGenerator(view: view)
                selectionGenerator = gen
                return gen
            }
            return UISelectionFeedbackGenerator()
        }

        let gen = UISelectionFeedbackGenerator()
        selectionGenerator = gen
        return gen
    }

    private static func notification() -> UINotificationFeedbackGenerator {
        if let cached = notificationGenerator { return cached }

        if #available(iOS 17.5, *) {
            if let view = bestEffortView() {
                let gen = UINotificationFeedbackGenerator(view: view)
                notificationGenerator = gen
                return gen
            }
            return UINotificationFeedbackGenerator()
        }

        let gen = UINotificationFeedbackGenerator()
        notificationGenerator = gen
        return gen
    }

    private static func bestEffortView() -> UIView? {
        let scenes = UIApplication.shared.connectedScenes.compactMap { $0 as? UIWindowScene }
        let ws = scenes.first(where: { $0.activationState == .foregroundActive }) ?? scenes.first

        let window = ws?.windows.first(where: { $0.isKeyWindow }) ?? ws?.windows.first
        return window?.rootViewController?.view ?? window
    }
}
