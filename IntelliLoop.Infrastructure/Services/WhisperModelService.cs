using IntelliLoop.Common.ExceptionMessages;
using IntelliLoop.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Whisper.net.Ggml;

namespace IntelliLoop.Infrastructure.Services
{
    public class WhisperModelService : IWhisperModelService
    {
        private readonly string _modelPath;
        public WhisperModelService(IConfiguration configuration)
        {
            var basepath = configuration["AppSettings:basePath"]
                ?? throw new InvalidOperationException(Configuration.BasePathNotConfigured);

            string whisperModelPath = configuration["Whisper:ModelPath"] ?? throw new InvalidOperationException(Configuration.WhisperModelPathNotConfigured);

            _modelPath = Path.Combine(basepath, whisperModelPath);
        }
        public async Task EnsureModelExistsAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(_modelPath))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);

            using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Small);

            await using var fileWriter = new FileStream(_modelPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

            await modelStream.CopyToAsync(fileWriter, cancellationToken);
        }
    }
}
