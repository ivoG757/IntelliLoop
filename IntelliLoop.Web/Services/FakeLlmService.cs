using IntelliLoop.Core.DTOs;
using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Core.Interfaces;

namespace IntelliLoop.Web.Services
{
    public class FakeLlmService : ILlmService
    {
        public Task<LectureAnalysis> AnalyzeLectureAsync(string transcript)
        {
            var analysis = new LectureAnalysis
            {
                Title = "Introduction to the Lecture",
                Summary = "This lecture introduces the main subject, explains its fundamental principles, and explores how the concepts can be applied in practice.",
                KeyConcepts = new List<string>
                {
                    "Fundamental principles",
                    "Core concepts and terminology",
                    "Practical applications",
                    "Relationships between concepts"
                },
                Notes = new List<NoteSection>
                {
                    new NoteSection
                    {
                        Title = "Introduction",
                        Content = "The lecture introduces the topic and explains why it is important."
                    },
                    new NoteSection
                    {
                        Title = "Main Concepts",
                        Content = "The main principles are explained alongside their definitions and relationships."
                    },
                    new NoteSection
                    {
                        Title = "Practical Applications",
                        Content = "The concepts can be applied to real-world situations to solve problems and improve understanding."
                    }
                }
            };

            return Task.FromResult(analysis);
        }
    }
}