using System.ComponentModel.DataAnnotations;

namespace IntelliLoop.Core.Entities.Models
{
    public class Lecture
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Summary { get; set; } = null!;
        public ICollection<string> KeyConcepts { get; set; } = new List<string>();
        public ICollection<NoteSection> Notes { get; set; } = new List<NoteSection>();
        Guid? ProcessingJobId { get; set; }
        ProcessingJob? ProcessingJob { get; set; }

        [Required]
        public string Transcript { get; set; } = null!;
    }
}
