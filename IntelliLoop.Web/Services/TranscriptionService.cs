using FFMpegCore;
using FFMpegCore.Pipes;
using IntelliLoop.Core.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Text;
using Whisper.net;
using Whisper.net.Ggml;

namespace IntelliLoop.Web.Services
{
    public class TranscriptionService : IDisposable
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

        public async Task<string> TranscribeAudio(string audioFilePath)
        {
            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException($"Audio file not found: {audioFilePath}");
            }

            await FFMpegArguments.FromFileInput(audioFilePath)
                .OutputToFile(audioFilePath + 1, overwrite: true,
                options => options
                .WithAudioSamplingRate(16000)
                .WithCustomArgument("-ac 1") // mono channel
                .WithAudioCodec("pcm_s16le")
                .ForceFormat("wav")
                ).ProcessAsynchronously();

            using var fileStream = File.OpenRead(audioFilePath + 1);

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
