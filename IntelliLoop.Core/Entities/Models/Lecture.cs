using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.Lecture;
namespace IntelliLoop.Core.Entities.Models
{
    public class Lecture
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [MaxLength(SummaryMaxLength)]
        public string Summary { get; set; } = null!;

        [MaxLength(ConceptMaxLength)]
        public ICollection<string> Concepts { get; set; } = new List<string>();
        public ICollection<NoteSection> Notes { get; set; } = new List<NoteSection>();
        public ProcessingJob ProcessingJob { get; set; } = null!;

        [Required]
        public string Transcript { get; set; } = null!;
    }
}
