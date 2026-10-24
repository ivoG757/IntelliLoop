using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.DTOs.Response
{
    public class NoteResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
