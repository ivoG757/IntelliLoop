using IntelliLoop.Core.Entities.Enums;

namespace IntelliLoop.Core.DTOs.Response
{
    public class JobResponseDto
    {
        public Guid JobId { get; set; }

        public string? Title { get; set; }

        public LectureGenerationStatus Status { get; set; }
    }
}
