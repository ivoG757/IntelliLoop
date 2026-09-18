using IntelliLoop.Core.Entities;

namespace IntelliLoop.Core.Repository
{
    public interface ILectureProcessingRepository
    {
        public Task<LectureGenerationJob> GetJobByIdAsync(string jobId);
        public Task AddJobAsync(ProcessingJob job);
        public Task UpdateJobAsync(ProcessingJob job);
        public Task<LectureGenerationJob> CreateJobAsync(string filePath);
    }
}
