using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Entities
{
    public class Lecture
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyConcepts { get; set; } = new();
        public List<NoteSection> Notes { get; set; } = new();
    }
}
