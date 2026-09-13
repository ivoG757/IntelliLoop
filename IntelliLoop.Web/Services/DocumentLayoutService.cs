using IntelliLoop.Core.Entities;

namespace IntelliLoop.Web.Services
{
    public class DocumentLayoutService
    {
        public string ToMarkdown(LectureAnalysis lecture)
        {
            return $"""
            # {lecture.Title}

            ## Summary

            {lecture.Summary}

            ## Key Concepts

            {string.Join("\n", lecture.KeyConcepts.Select(x => $"- {x}"))}

            ## Notes

            {string.Join("\n\n", lecture.Notes.Select(x =>
                $"### {x.Title}\n{x.Content}"))}
            """;
        }
    }
}