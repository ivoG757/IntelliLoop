using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface ILectureProcessingRepository
    {
        public Task<ProcessingJob?> GetJobByIdAsync(Guid jobId, Guid userId);
        public Task<ProcessingJob?> GetJobByIdAsync(Guid jobId);
        public Task<Lecture?> GetLectureByJobIdAsync(Guid jobId, Guid userId);
        public Task AddJobAsync(ProcessingJob job);
        public Task<IEnumerable<ProcessingJob>> GetJobsByUserIdAsync(Guid userId, int skip, int take);
    }
}
