using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.NoteSection;
namespace IntelliLoop.Core.DTOs
{
    public class NoteAnalysis
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;
    }
}
