namespace IntelliLoop.Core.DTOs.Request
{
    public class UpdateLectureRequestDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public Guid UserId { get; set; }

        public string Summary { get; set; } = null!;

        public ICollection<string> Concepts { get; set; } = new List<string>();

        public ICollection<UpdateNotesRequestDto> Notes { get; set; } = new List<UpdateNotesRequestDto>();
    }
}
