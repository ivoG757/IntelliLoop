using IntelliLoop.Core.Entities.Enums;

namespace IntelliLoop.Web.Models
{
    public class ProcessingJobViewModel
    {
        public Guid Id { get; set; }
        public LectureGenerationStatus Status { get; set; }
    }
}
