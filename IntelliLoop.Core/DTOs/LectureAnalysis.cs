using IntelliLoop.Core.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelliLoop.Core.DTOs
{
    public class LectureAnalysis
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyConcepts { get; set; } = new();
        public List<NoteSection> Notes { get; set; } = new();
    }
}
