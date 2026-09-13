using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Interfaces;
using Microsoft.Extensions.AI;

namespace IntelliLoop.Web.Services
{
    public class LocalLlmService : ILlmService
    {
        private readonly IChatClient _client;

        public LocalLlmService(IChatClient client)
        {
            _client = client;
        }

        public async Task<Lecture> AnalyzeLectureAsync(string transcript)
        {
            var message = new ChatMessage(ChatRole.User, $"Summarize this lecture in 3 sentences:\n\n{transcript}");
            var response = await _client.GetResponseAsync(message);
            return new Lecture
            {
                Title = "Analyzed Lecture",
                Transcript = response.Text
            };
        }
    }
}
