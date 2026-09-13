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

        public async Task<LectureAnalysis> AnalyzeLectureAsync(string transcript)
        {
            var messages = new[]
            {
                new ChatMessage(ChatRole.System,
                    """
                    You are IntelliLoop, an AI study assistant that transforms lecture transcripts into detailed, useful study notes.

                    Analyze the entire lecture transcript carefully and extract as much useful information as the transcript supports.

                    Rules:

                    * Do not omit useful information simply to keep the answer short.
                    * Make every field as informative and detailed as possible.
                    * Never leave a field empty when information relevant to that field exists in the transcript.
                    * Use only information supported by the transcript. Do not invent facts, examples, definitions, relationships, or conclusions.
                    * Preserve important technical details, terminology, distinctions, examples, explanations, processes, relationships, and practical information.
                    * Organize related information together so that the result is easy to study.
                    * Prefer completeness over brevity, while avoiding repetition and meaningless filler.
                    * Write explanations clearly enough that a student could study from the generated notes without having to reread the original transcript.

                    Title:

                    * Create a short, descriptive title that accurately represents the lecture.

                    Summary:

                    * Give a detailed but clear summary of the entire lecture.
                    * Include the main ideas, important explanations, and major relationships between concepts.

                    KeyConcepts:

                    * Extract all important concepts, technologies, terms, principles, and ideas.
                    * Include enough concepts to cover the important material without adding irrelevant items.
                    * Keep each concept concise.

                    BulletPoints:

                    * Divide the lecture into its important topics or sections.
                    * Create as many sections as necessary to cover the useful information.
                    * Each section must have a descriptive Header.
                    * Each section must contain detailed Paragraphs explaining the topic.
                    * Include definitions, explanations, examples, comparisons, relationships, and important technical details when they are present in the transcript.
                    * Do not make Paragraphs artificially short. Combine related information into coherent explanations.
                    * Do not leave BulletPoints empty when the transcript contains material that can be organized into sections.

                    The goal is to produce comprehensive, structured study notes from the lecture, not merely a short summary.
                    
                    """), // TODO: Consider moving this prompt to a separate file for easier maintenance and editing

                new ChatMessage(ChatRole.User, $"Analyze this lecture:\n\n{transcript}")};

            var response = await _client.GetResponseAsync<LectureAnalysis>(messages);

            return response.Result;
        }
    }
}
