using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Infrastructure.Configuration;
using IntelliLoop.Infrastructure.Services;
using OllamaSharp;

namespace IntelliLoop.Web.Extensions
{
    public static class OllamaExtensions
    {
        public static IServiceCollection AddOllama(this IServiceCollection services, IConfiguration configuration)
        {
            var timeoutMinutes = configuration.GetValue<int?>("Ollama:Timeout") ?? 15;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["Ollama:BaseUrl"] ?? throw new InvalidOperationException("Ollama base URL is not configured.")),
                Timeout = TimeSpan.FromMinutes(timeoutMinutes)
            };

            var ollamaClient = new OllamaApiClient(httpClient)
            {
                SelectedModel = configuration["Ollama:Model"] ?? throw new InvalidOperationException("Ollama model is not configured.")
            };

            services.AddChatClient(ollamaClient);

            services.AddScoped<ILlmService, LocalLlmService>();

            services.AddOptions<LocalLlmOptions>()
                .Bind(configuration.GetSection(LocalLlmOptions.SectionName))
                .ValidateDataAnnotations()

                .Validate(options => options.MinimumContextTokens <= options.MaximumContextTokens, 
                    "LocalLlm:MinimumContextTokens must not exceed MaximumContextTokens.")

                .Validate(options => options.ExtractionOutputReserveTokens + options.ContextSafetyMarginTokens < options.MaximumContextTokens,
                    "The extraction output reserve plus safety margin must be less than MaximumContextTokens.")

                .Validate(options => options.FormattingOutputReserveTokens + options.ContextSafetyMarginTokens < options.MaximumContextTokens,
                    "The formatting output reserve plus safety margin must be less than MaximumContextTokens.")
                .ValidateOnStart();

            return services;
        }
    }
}
