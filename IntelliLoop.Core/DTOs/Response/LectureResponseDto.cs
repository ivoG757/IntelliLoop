namespace IntelliLoop.Core.DTOs.Response
{
    public class LectureResponseDto
    {
        public string Title { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Summary { get; set; } = null!;
        public ICollection<string> Concepts { get; set; } = new List<string>();
        public ICollection<NoteResponseDto> Notes { get; set; } = new List<NoteResponseDto>();
        public Guid JobId { get; set; }
        public string Transcript { get; set; } = null!;
    }
}
