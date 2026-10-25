using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Infrastructure.Services;
using OllamaSharp;

namespace IntelliLoop.Web.Extensions
{
    public static class OllamaExtensions
    {
        public static IServiceCollection AddOllama(this IServiceCollection services, IConfiguration configuration)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["Ollama:BaseUrl"] ?? throw new InvalidOperationException("Ollama base URL is not configured.")),
                Timeout = TimeSpan.FromMinutes(10)
            };

            var ollamaClient = new OllamaApiClient(httpClient)
            {
                SelectedModel = configuration["Ollama:Model"] ?? throw new InvalidOperationException("Ollama model is not configured.")
            };

            services.AddChatClient(ollamaClient);
            services.AddScoped<ILlmService, LocalLlmService>();

            return services;
        }
    }
}
