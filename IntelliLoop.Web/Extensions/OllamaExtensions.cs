using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Services;
using OllamaSharp;

namespace IntelliLoop.Web.Extensions
{
    public static class OllamaExtensions
    {
        public static IServiceCollection AddOllama(this IServiceCollection services)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(10)
            };

            var ollamaClient = new OllamaApiClient(httpClient)
            {
                SelectedModel = "qwen3:8b"
            };
            // TODO: Move this to a configuration file or environment variable later

            services.AddChatClient(ollamaClient);
            services.AddScoped<ILlmService, LocalLlmService>();

            return services;
        }
    }
}
