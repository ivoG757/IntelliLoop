using static IntelliLoop.Common.ExceptionMessages.Configuration;
using IntelliLoop.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Whisper.net.Ggml;

namespace IntelliLoop.Infrastructure.Services
{
    public class WhisperModelService : IWhisperModelService
    {
        private readonly string _modelPath;
        private readonly string _modelName;
        public WhisperModelService(IConfiguration configuration)
        {
            var basepath = configuration["AppSettings:basePath"]
                ?? throw new InvalidOperationException(BasePathNotConfigured);

            string whisperModelPath = configuration["Whisper:ModelPath"] 
                ?? throw new InvalidOperationException(WhisperModelPathNotConfigured);


            _modelName = configuration["Whisper:Model"] 
                ?? throw new InvalidOperationException(WhisperModelNotConfigured);

            _modelPath = Path.Combine(basepath, whisperModelPath);
        }
        public async Task EnsureModelExistsAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(_modelPath))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);

            using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(MapModelName());

            await using var fileWriter = new FileStream(_modelPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

            await modelStream.CopyToAsync(fileWriter, cancellationToken);
        }

        private GgmlType MapModelName()
        {
            return _modelName.Trim().ToLowerInvariant() 
                switch
            {
                "tiny" => GgmlType.Tiny,
                "base" => GgmlType.Base,
                "small" => GgmlType.Small,
                "medium" => GgmlType.Medium,
                "largev1" => GgmlType.LargeV1,
                _ => throw new InvalidOperationException(
                    $"Unsupported Whisper model '{_modelName}'.")
            };
        }
    }
}
