using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliLoop.Core.Entities
{
    public class ProcessingJob
    {
        [Key]
        public Guid Id { get; set; }
        public string FilePath { get; set; } = null!;
        public LectureGenerationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(Lecture))]
        public Guid? LectureId { get; set; }
        public Lecture? Lecture { get; set; }
    }
}
