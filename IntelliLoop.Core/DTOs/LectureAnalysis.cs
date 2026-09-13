using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.DTOs
{
    public class LectureAnalysis
    {
        public string Summary { get; set; } = null!;
        public List<string> Topics { get; set; } = new List<string>();
        public List<string> KeyConcepts { get; set; } = new List<string>();
        public List<string> ImportantPoints { get; set; } = new List<string>();
        public List<string> ReviewQuestions { get; set; } = new List<string>();
    }
}
