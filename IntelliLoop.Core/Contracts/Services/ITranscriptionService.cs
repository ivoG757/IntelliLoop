namespace IntelliLoop.Core.Contracts.Services
{
    public interface ITranscriptionService
    {
        public Task<string> TranscribeAudioAsync(string audioFilePath, CancellationToken token);
    }
}
