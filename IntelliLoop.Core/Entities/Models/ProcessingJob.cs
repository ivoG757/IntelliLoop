using IntelliLoop.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliLoop.Core.Entities.Models
{
    public class ProcessingJob
    {
        [Key]
        public Guid Id { get; set; }
        public string FilePath { get; set; } = null!;
        public LectureGenerationStatus Status { get; set; }
        public LectureGenerationStatus? FailedAtStage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(Lecture))]
        public Guid? LectureId { get; set; }
        public Lecture? Lecture { get; set; }
    }
}
