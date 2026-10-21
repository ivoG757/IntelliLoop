using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.DTOs.Response
{
    public class AudioResponseDto
    {
        public Stream Content { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
