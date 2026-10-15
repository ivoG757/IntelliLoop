using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace IntelliLoop.Infrastructure.Services
{
    public class LocalLlmService : ILlmService
    {
        private readonly IChatClient _client;
        private readonly IPromptService _promptService;
        public LocalLlmService(IChatClient client, ILogger<LocalLlmService> logger, IConfiguration configuration, IPromptService promptService)
        {
            _client = client;
            _promptService = promptService;
        }

        public async Task<LectureAnalysis> AnalyzeLectureAsync(string transcript)
        {

            var extractionPrompts = _promptService.GetExtractionPrompts();

            var extractionMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, extractionPrompts["system"][0]),
                new ChatMessage(ChatRole.User, extractionPrompts["user"][0] + $"{Environment.NewLine}{transcript}")
            };

            var analyzedInfo = await _client.GetResponseAsync(extractionMessages);

            var formattingPrompts = _promptService.GetFormattingPrompts();

            var formattingMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, formattingPrompts["system"][0]),
                new ChatMessage(ChatRole.User, formattingPrompts["user"][0] + $"{Environment.NewLine}{analyzedInfo.Text}")
            };

            var structuredInfo = await _client.GetResponseAsync<LectureAnalysis>(formattingMessages);

            return structuredInfo.Result;
        }
    }
}
