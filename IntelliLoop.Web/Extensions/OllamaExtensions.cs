using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Services;
using OllamaSharp;

namespace IntelliLoop.Web.Extensions
{
    public static class OllamaExtensions
    {
        public static IServiceCollection AddOllama(this IServiceCollection services, IConfiguration configuration)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(10)
            };

            var ollamaClient = new OllamaApiClient(httpClient)
            {
                SelectedModel = configuration["Ollama:Model"] ?? throw new InvalidOperationException("Ollama model is not configured.")
            };

            services.AddChatClient(ollamaClient);
            services.AddScoped<ILlmService, FakeLlmService>(); //REMINDER: Add the real LLM service implementation later

            return services;
        }
    }
}
