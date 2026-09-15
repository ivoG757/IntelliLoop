using System.ComponentModel.DataAnnotations;

namespace IntelliLoop.Core.Entities
{
    public class Lecture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Summary { get; set; } = null!;
        public ICollection<string> KeyConcepts { get; set; } = new List<string>();
        public ICollection<NoteSection> Notes { get; set; } = new List<NoteSection>();
    }
}
