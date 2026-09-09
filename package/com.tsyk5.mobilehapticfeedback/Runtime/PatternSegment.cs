namespace tsyk5.MobileHapticFeedback
{
    /// <summary>
    /// One segment of a PlayPattern call: how long, how strong (0 = silence), and how sharp.
    /// </summary>
    public readonly struct PatternSegment
    {
        /// <summary>Sharpness used when a segment does not specify one.</summary>
        public const float DefaultSharpness = 0.5f;

        public float DurationSec { get; }
        public float Amplitude { get; }

        /// <summary>
        /// Crispness of the segment (0..1). Honored where <see cref="MobileHapticFeedback.IsSharpnessSupported"/>
        /// is true; ignored elsewhere.
        /// </summary>
        public float Sharpness { get; }

        public PatternSegment(float durationSec, float amplitude, float sharpness = DefaultSharpness)
        {
            DurationSec = durationSec;
            Amplitude = amplitude;
            Sharpness = sharpness;
        }
    }
}
