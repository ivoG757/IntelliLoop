using IntelliLoop.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Entities
{
    public class LectureGenerationJob
    {
        public string Id { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public Enums.LectureGenerationStatus Status { get; set; } = LectureGenerationStatus.Queued;
    }
}
