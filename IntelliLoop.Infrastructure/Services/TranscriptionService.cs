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

        private readonly WhisperProcessor _processor;

        public TranscriptionService(IConfiguration configuration)
        {

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

        public async Task<string> TranscribeAudioAsync(string audioFilePath)
        {
            if (!File.Exists(audioFilePath))
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

                await using var fileStream = File.OpenRead(convertedFilePath);

                var transcript = new StringBuilder();

                await foreach (var result in _processor.ProcessAsync(fileStream))
                {
                    var text = $"{result.Start}->{result.End}: {result.Text}"; //TODO: Add timestamp formatting, also add it to constants

                    transcript.AppendLine(text);
                }

                return transcript.ToString();
            }

            finally
            {
                if (File.Exists(convertedFilePath))
                {
                    File.Delete(convertedFilePath);
                }
            }
        }
    }
}
