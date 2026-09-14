using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Interfaces;
using Microsoft.Extensions.AI;
using System.Text;

namespace IntelliLoop.Web.Services
{
    public class LocalLlmService : ILlmService
    {
        private readonly IChatClient _client;
        private readonly ILogger<LocalLlmService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPromptService _promptService;
        public LocalLlmService(IChatClient client, ILogger<LocalLlmService> logger, IConfiguration configuration, IPromptService promptService)
        {
            _client = client;
            _logger = logger;
            _configuration = configuration;
            _promptService = promptService;
        }

        public async Task<LectureAnalysis> AnalyzeLectureAsync(string transcript)
        {
            List<ChatMessage> systemMessages = new List<ChatMessage>();

            systemMessages.Add(new ChatMessage(ChatRole.System, _promptService.GetPrompts()));

            systemMessages.Add(new ChatMessage(ChatRole.User, $"Analyze this lecture:\n\n{transcript}"));

            var response = await _client.GetResponseAsync<LectureAnalysis>(systemMessages);

            return response.Result;
        }
    }
}
