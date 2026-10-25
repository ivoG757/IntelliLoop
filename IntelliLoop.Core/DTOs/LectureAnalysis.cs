using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.Lecture;
namespace IntelliLoop.Core.DTOs
{
    public class LectureAnalysis
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(SummaryMaxLength)]
        public string Summary { get; set; } = string.Empty;

        [MaxLength(ConceptMaxLength)]
        public List<string> KeyConcepts { get; set; } = new();
        public List<NoteAnalysis> Notes { get; set; } = new();
    }
}
