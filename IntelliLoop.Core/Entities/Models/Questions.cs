using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliLoop.Core.Entities.Models
{
    public class Questions
    {
        [Key]
        public int id { get; set; }

        [ForeignKey(nameof(Lecture))]
        public int LectureId { get; set; }
        public Lecture lecture { get; set; } = null!;

        [Required]
        public List<string> QuestionsList { get; set; } = null!;
    }
}
