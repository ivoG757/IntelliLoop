using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Web.Models;

namespace IntelliLoop.Web.Mapping;

public static class StatusManager
{
    private static readonly Dictionary<LectureGenerationStatus, string> StatusMessages = new()
    {
        [LectureGenerationStatus.Queued] = "Waiting to process",
        [LectureGenerationStatus.Transcribing] = "Transcribing audio",
        [LectureGenerationStatus.Extracting] = "Generating notes",
        [LectureGenerationStatus.Completed] = "Complete lecture"
    };

    public static string Map(LectureGenerationStatus status)
    {
        return StatusMessages.TryGetValue(status, out var message) ? message : status.ToString();
    }
    public static List<string> GetSteps(LectureGenerationStatus status)
    {
        return StatusMessages.Values.ToList();
    }
}