using IntelliLoop.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
    }
}
