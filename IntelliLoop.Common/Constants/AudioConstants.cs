namespace IntelliLoop.Common.Constants
{
    public static class AudioConstants
    {
        public const string WavContentType = "audio/wav";
        public const string WavExtension = "wav";
        public const int WhisperSampleRate = 16000;
        public const string MonoAudioArgument = "-ac 1";
        public const string PcmAudioCodec = "pcm_s16le";
        public const string TempFileSuffix = ".temp";
        public static readonly long MaxAudioFileSizeBytes = 100L * 1024 * 1024;

        public static readonly HashSet<string> AllowedAudioExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3",
            ".wav",
            ".m4a",
            ".ogg",
            ".webm"
        };
    }
}
