namespace IntelliLoop.Web.Common.Constants
{
    public static class AudioConstants
    {
        public const string WavContentType = "audio/wav";
        public const string WavExtension = ".wav";
        public const int WhisperSampleRate = 16000;
        public const string MonoAudioArgument = "-ac 1";
        public const string PcmAudioCodec = "pcm_s16le";
        public const string TempFileSuffix = ".temp";
    }
}
