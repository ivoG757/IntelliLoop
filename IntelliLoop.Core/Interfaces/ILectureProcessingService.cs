namespace IntelliLoop.Core.Interfaces
{
    public interface ILectureProcessingService
    {
        public Task ProcessLectureAsync(Guid jobId);
    }
}
