namespace IntelliLoop.Core.Contracts.Services
{
    public interface ILectureProcessingService
    {
        public Task ProcessLectureAsync(Guid jobId);
    }
}
