using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliLoop.Core.Entities.Models
{
    public class NoteSection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [ForeignKey(nameof(Lecture))]
        public int LectureId { get; set; }
        public Lecture lecture { get; set; } = null!;
    }
}
