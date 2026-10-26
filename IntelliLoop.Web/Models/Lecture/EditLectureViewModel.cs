using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.Lecture;
namespace IntelliLoop.Web.Models.Lecture
{
    public class EditLectureViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(SummaryMaxLength)]
        public string Summary { get; set; } = null!;

        [MaxLength(ConceptTextMaxLength)]
        public List<string> KeyConcepts { get; set; } = new List<string>();
        public List<EditNoteViewModel> Notes { get; set; } = new List<EditNoteViewModel>();
    }
}
