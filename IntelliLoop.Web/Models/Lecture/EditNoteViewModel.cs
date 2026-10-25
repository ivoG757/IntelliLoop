using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.NoteSection;
namespace IntelliLoop.Web.Models.Lecture
{
    public class EditNoteViewModel
    {
        public Guid? Id { get; set; }

        [StringLength(TitleMaxLength)]
        public string? Title { get; set; }

        [StringLength(ContentMaxLength)]
        public string? Content { get; set; }

    }
}
