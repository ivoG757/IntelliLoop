using IntelliLoop.Core.Entities;

namespace IntelliLoop.Core.Repository
{
    public interface ILectureProcessingRepository
    {
        public Task<ProcessingJob?> GetJobByIdAsync(Guid jobId);
        public Task AddJobAsync(ProcessingJob job);
    }
}
