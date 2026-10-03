using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Web.Models
{
    public class LectureViewModel
    {
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public ICollection<string> KeyConcepts { get; set; } = new List<string>();
        public ICollection<NoteViewModel> Notes { get; set; } = new List<NoteViewModel>();
        public string Transcript { get; set; } = null!;
    }
}
