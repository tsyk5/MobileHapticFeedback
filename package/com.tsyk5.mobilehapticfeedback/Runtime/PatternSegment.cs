namespace tsyk5.MobileHapticFeedback
{
    /// <summary>
    /// One segment of a PlayPattern call: how long, and how strong (0 = silence).
    /// </summary>
    public readonly struct PatternSegment
    {
        public float DurationSec { get; }
        public float Amplitude { get; }

        public PatternSegment(float durationSec, float amplitude)
        {
            DurationSec = durationSec;
            Amplitude = amplitude;
        }
    }
}
