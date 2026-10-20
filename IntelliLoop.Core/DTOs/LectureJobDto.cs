using IntelliLoop.Core.Entities.Enums;

namespace IntelliLoop.Core.DTOs
{
    public class LectureProcessingJobResponseDto
    {
        public Guid JobId { get; set; }

        public string? Title { get; set; }

        public LectureGenerationStatus Status { get; set; }
    }
}
