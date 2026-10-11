using FFMpegCore;
using IntelliLoop.Core.Interfaces;
using System.Text;
using Whisper.net;
using static IntelliLoop.Web.Common.Constants.AudioConstants;

namespace IntelliLoop.Web.Services
{
    public class TranscriptionService : ITranscriptionService, IDisposable
    {

        private readonly WhisperFactory _whisperFactory;

        private readonly WhisperProcessor _processor;

        private readonly string _modelPath;

        public TranscriptionService(IConfiguration configuration)
        {

            _modelPath = configuration["Whisper:ModelPath"] 
                ?? throw new InvalidOperationException("Whisper model path is not configured!");

            _whisperFactory = WhisperFactory.FromPath(_modelPath);

            _processor = _whisperFactory.CreateBuilder().WithLanguage(configuration["Whisper:Language"] ?? "auto").Build(); 
        }

        public void Dispose()
        {
            _whisperFactory?.Dispose();
            _processor?.Dispose();
        }

        public async Task<string> TranscribeAudioAsync(string audioFilePath)
        {
            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException($"Audio file not found: {audioFilePath}");
            }

            await FFMpegArguments.FromFileInput(audioFilePath)
                .OutputToFile(audioFilePath + TempFileSuffix, overwrite: true,
                options => options
                .WithAudioSamplingRate(WhisperSampleRate)
                .WithCustomArgument(MonoAudioArgument)
                .WithAudioCodec(PcmAudioCodec)
                .ForceFormat(WavExtension)
                ).ProcessAsynchronously();

            using var fileStream = File.OpenRead(audioFilePath + TempFileSuffix);

            var transcript = new StringBuilder();

            await foreach (var result in _processor.ProcessAsync(fileStream))
            {
                var text = $"{result.Start}->{result.End}: {result.Text}";
                Console.WriteLine(text);
                transcript.AppendLine(text);
            }

            return transcript.ToString();
        }
    }
}
