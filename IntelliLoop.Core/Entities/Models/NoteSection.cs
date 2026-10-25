using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static IntelliLoop.Common.Constants.ValidationConstants.NoteSection;
namespace IntelliLoop.Core.Entities.Models
{
    public class NoteSection
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;

        [ForeignKey(nameof(Lecture))]
        public Guid LectureId { get; set; }
        public Lecture Lecture { get; set; } = null!;
    }
}
