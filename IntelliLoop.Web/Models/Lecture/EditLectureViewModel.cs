using System.ComponentModel.DataAnnotations;

namespace IntelliLoop.Web.Models.Lecture
{
    public class EditLectureViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Summary { get; set; } = null!;
        public List<string> KeyConcepts { get; set; } = new List<string>();
        public List<EditNoteViewModel> Notes { get; set; } = new List<EditNoteViewModel>();
    }
}
