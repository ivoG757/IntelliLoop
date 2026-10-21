namespace IntelliLoop.Core.DTOs.Request
{
    public class UploadLectureRequestDto
    {
        public Guid UserId { get; set; }
        public Stream Content { get; set; } = Stream.Null;
        public string FileName { get; set; } = string.Empty;
    }
}