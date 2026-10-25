using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs;
using Microsoft.Extensions.AI;
using System.ComponentModel.DataAnnotations;
namespace IntelliLoop.Infrastructure.Services
{
    public class LocalLlmService : ILlmService
    {
        private readonly IChatClient _client;
        private readonly IPromptService _promptService;
        public LocalLlmService(IChatClient client, IPromptService promptService)
        {
            _client = client;
            _promptService = promptService;
        }

        public async Task<LectureAnalysis> AnalyzeLectureAsync(string transcript, CancellationToken token)
        {

            var extractionPrompts = _promptService.GetExtractionPrompts();

            var extractionMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, extractionPrompts["system"][0]),
                new ChatMessage(ChatRole.User, extractionPrompts["user"][0] + $"{Environment.NewLine}{transcript}")
            };

            var analyzedInfo = await _client.GetResponseAsync(extractionMessages, null, token);

            var formattingPrompts = _promptService.GetFormattingPrompts();

            var formattingMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, formattingPrompts["system"][0]),
                new ChatMessage(ChatRole.User, formattingPrompts["user"][0] + $"{Environment.NewLine}{analyzedInfo.Text}")
            };

            var structuredInfo = await _client.GetResponseAsync<LectureAnalysis>
                (formattingMessages, null, useJsonSchemaResponseFormat: false, token);

            var result = structuredInfo.Result;

            if (!IsAnalysisInConstraints(result))
            {
                throw new ValidationException("The generated lecture does not meet the requirements");
            }
            return result;

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
