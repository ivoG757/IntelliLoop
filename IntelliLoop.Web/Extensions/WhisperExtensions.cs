using Whisper.net.Ggml;

namespace IntelliLoop.Web.Extensions
{
    public static class WhisperExtensions
    {
        public static async Task<IServiceCollection> ConfigureWhisperAsync(this IServiceCollection services, IConfiguration configuration)
        {
            if (!File.Exists(configuration["Whisper:ModelPath"]))
            {
                using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Small); //downloads the small model of whisper
                using var fileWriter = File.Create(configuration["Whisper:ModelPath"]!);
                await modelStream.CopyToAsync(fileWriter);
            }

            return services;
        }
    }
}
