using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs;
using IntelliLoop.Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using OllamaChatRequest = OllamaSharp.Models.Chat.ChatRequest;
using OllamaRequestOptions = OllamaSharp.Models.RequestOptions;

namespace IntelliLoop.Infrastructure.Services
{ //TODO: write a documentation for this service
    public class LocalLlmService : ILlmService
    {
        private readonly IChatClient _client;
        private readonly IPromptService _promptService;
        private readonly ILogger<LocalLlmService> _logger;
        private readonly LocalLlmOptions _configuration;

        public LocalLlmService(
            IChatClient client,
            IPromptService promptService,
            ILogger<LocalLlmService> logger,
            IOptions<LocalLlmOptions> options)
        {
            _client = client;
            _promptService = promptService;
            _logger = logger;
            _configuration = options.Value;
        }

        public async Task<LectureAnalysis> AnalyzeLectureAsync(string transcript, CancellationToken token)
        {
            var extractionPrompts = _promptService.GetExtractionPrompts();

            var extractionMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, extractionPrompts["system"][0]),
                new ChatMessage(ChatRole.User, extractionPrompts["user"][0] + $"{Environment.NewLine}{transcript}")
            };

            ChatOptions extractionOptions = BuildOptions(
                extractionMessages,
                _configuration,
                _configuration.ExtractionOutputReserveTokens,
                out var extractionCtx);

            var analyzedInfo = await _client.GetResponseAsync(extractionMessages, extractionOptions, token);
            CheckResponse("extraction", analyzedInfo, extractionCtx);

            if (string.IsNullOrWhiteSpace(analyzedInfo.Text))
            {
                throw new InvalidOperationException("Extraction returned an empty answer.");
            }

            var formattingPrompts = _promptService.GetFormattingPrompts();

            var formattingMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, formattingPrompts["system"][0]),
                new ChatMessage(ChatRole.User, formattingPrompts["user"][0] + $"{Environment.NewLine}{analyzedInfo.Text}")
            };

            var formattingOptions = BuildOptions(
                formattingMessages,
                _configuration,
                _configuration.FormattingOutputReserveTokens,
                out var formattingCtx);

            var structuredInfo = await _client.GetResponseAsync<LectureAnalysis>(
                formattingMessages, formattingOptions, useJsonSchemaResponseFormat: _configuration.UseJsonSchemaResponseFormat, token); 

            CheckResponse("formatting", structuredInfo, formattingCtx);

            var result = structuredInfo.Result;

            if (!IsAnalysisInConstraints(result))
            {
                throw new ValidationException("The generated lecture does not meet the requirements");
            }
            return result;
        }

        /// <summary>
        /// Builds chat options based on the provided messages, context limits, and temperature.
        /// rounds the result up, and passes that context size to Ollama.
        /// </summary>
        /// <param name="messages">The collection of chat messages to analyze.</param>
        /// <param name="options">The llm configuration options.</param>
        /// <param name="outputReserve">The number of tokens to reserve for output.</param>
        /// <param name="numContext">When this method returns, contains the calculated context size in tokens.</param>
        /// <returns>A ChatOptions instance configured with the specified parameters.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the required context exceeds the maximum allowed context.</exception>
        private static ChatOptions BuildOptions(IEnumerable<ChatMessage> messages, LocalLlmOptions options, int outputReserve, out int numContext)
        {
            var promptTokens = messages.Sum(m => m.Text.Length) / options.CharactersPerTokenEstimate;
            var needed = promptTokens + outputReserve;

            var context = Math.Max(options.MinimumContextTokens, 
                ((needed + options.ContextRoundingMultipleTokens - 1) / options.ContextRoundingMultipleTokens) * options.ContextRoundingMultipleTokens);

            if (context > options.MaximumContextTokens)
            {
                throw new InvalidOperationException(
                    $"The text needs about {context} tokens of context but MaxContext is {options.MaximumContextTokens}. " +
                    "Raise MaxContext (uses more graphics card memory) or split the transcript.");
            }

            numContext = context;

            return new ChatOptions
            {
                Temperature = options.Temperature,

                RawRepresentationFactory = _ => new OllamaChatRequest
                {
                    Think = options.EnableThinking,
                    Options = new OllamaRequestOptions { NumCtx = context }
                }
            };
        }

        private void CheckResponse(string stage, ChatResponse response, int numContext)
        {
            _logger.LogInformation(
                "{Stage}: context {Ctx}, input tokens {In}, output tokens {Out}, finish reason {Finish}",
                stage, numContext, response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount, response.FinishReason);

            if (response.FinishReason == ChatFinishReason.Length)
            {
                throw new InvalidOperationException(
                    $"{stage}: the model's answer was cut off. Raise the output reserve.");
            }

            if (response.Usage?.InputTokenCount is long inputTokens && inputTokens >= numContext - _configuration.ContextSafetyMarginTokens)
            {
                throw new InvalidOperationException(
                    $"{stage}: the context window is almost completely full, so the text was probably cut off. " +
                    "Lower CharsPerToken or raise MaxContext.");
            }
        }

        private static bool IsAnalysisInConstraints(LectureAnalysis analysis)
        {

            if (analysis.KeyConcepts is null || analysis.Notes is null)
            {
                return false;
            }

            foreach (var concept in analysis.KeyConcepts)
            {
                if (string.IsNullOrWhiteSpace(concept))
                {
                    return false;
                }
            }

            var results = new List<ValidationResult>();

            foreach (var note in analysis.Notes)
            {
                if (note is null)
                {
                    return false;
                }

                results.Clear();

                if (!Validator.TryValidateObject
                    (note, new ValidationContext(note), results, validateAllProperties: true))
                {
                    return false;
                }
            }
            return Validator.TryValidateObject(analysis, new ValidationContext(analysis), results, validateAllProperties: true);
        }
    }
}