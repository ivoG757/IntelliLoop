using System.ComponentModel.DataAnnotations;
using static IntelliLoop.Common.Constants.ValidationConstants.NoteSection;
namespace IntelliLoop.Web.Models.Lecture
{
    public class EditNoteViewModel
    {
        public Guid? Id { get; set; }

        [MaxLength(TitleMaxLength)]
        public string? Title { get; set; }

        [MaxLength(ContentMaxLength)]
        public string? Content { get; set; }

    }
}
