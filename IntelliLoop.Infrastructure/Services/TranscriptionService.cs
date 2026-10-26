using FFMpegCore;
using IntelliLoop.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using Whisper.net;
using static IntelliLoop.Common.Constants.AudioConstants;
using static IntelliLoop.Common.ExceptionMessages.Configuration;

namespace IntelliLoop.Infrastructure.Services
{
    public class TranscriptionService : ITranscriptionService, IDisposable
    {

        private readonly WhisperFactory _whisperFactory;
        private readonly IFileStorage _storage;
        private readonly WhisperProcessor _processor;

        public TranscriptionService(IConfiguration configuration, IFileStorage storage)
        {
            _storage = storage;

            var modelPath = configuration["Whisper:ModelPath"] 
                ?? throw new InvalidOperationException(WhisperModelPathNotConfigured);

            var basePath = configuration["AppSettings:basePath"]
                ?? throw new InvalidOperationException(BasePathNotConfigured);

            _whisperFactory = WhisperFactory.FromPath(Path.Combine(basePath, modelPath));

            _processor = _whisperFactory.CreateBuilder().WithLanguage(configuration["Whisper:Language"] ?? "auto").Build(); 
        }

        public void Dispose()
        {
            _processor?.Dispose();
            _whisperFactory?.Dispose();
        }

        public async Task<string> TranscribeAudioAsync(string audioFilePath, CancellationToken token)
        {
            if (!_storage.Exists(audioFilePath))
            {
                throw new FileNotFoundException($"Audio file not found: {audioFilePath}");
            }

            var convertedFilePath = audioFilePath + TempFileSuffix;

            try
            {
                await FFMpegArguments.FromFileInput(audioFilePath)
                    .OutputToFile(convertedFilePath, overwrite: true,
                    options => 
                    options.WithAudioSamplingRate(WhisperSampleRate)
                    .WithCustomArgument(MonoAudioArgument)
                    .WithAudioCodec(PcmAudioCodec)
                    .ForceFormat(WavExtension)
                    ).ProcessAsynchronously();

                await using var fileStream = _storage.GetStream(convertedFilePath);

                var transcript = new StringBuilder();

                await foreach (var result in _processor.ProcessAsync(fileStream, token))
                {
                    var text = $"{result.Text}";

                    transcript.AppendLine(text);
                }

                return transcript.ToString();
            }

            finally
            {
                if (_storage.Exists(convertedFilePath))
                {
                    _storage.Delete(convertedFilePath);
                }
            }
        }
    }
}
