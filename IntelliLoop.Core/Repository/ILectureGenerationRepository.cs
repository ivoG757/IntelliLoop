using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Repository
{
    public interface ILectureProcessingRepository
    {
        public Task<ProcessingJob?> GetJobByIdAsync(Guid jobId);
        public Task<Lecture?> GetLectureByJobIdAsync(Guid jobId);
        public Task AddJobAsync(ProcessingJob job);
    }
}
