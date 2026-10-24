namespace IntelliLoop.Core.DTOs.Request
{
    public class UpdateNotesRequestDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
