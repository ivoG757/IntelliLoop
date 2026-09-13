using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelliLoop.Core.Entities
{
    public class Lecture
    {
        [Required]
        public string Title { get; set; } 
        public string Transcript { get; set; } = string.Empty;
    }
}
