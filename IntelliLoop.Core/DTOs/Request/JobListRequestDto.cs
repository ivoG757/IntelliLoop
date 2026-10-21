namespace IntelliLoop.Core.DTOs.Request
{
    public class JobListRequestDto
    {
        public Guid UserId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}
