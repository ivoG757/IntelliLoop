using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface ILectureProcessingRepository
    {
        public Task<ProcessingJob?> GetJobByIdAsync(Guid jobId);
        public Task<Lecture?> GetLectureByJobIdAsync(Guid jobId);
        public Task<string> GetAudioFilePathByJobIdAsync(Guid jobId);
        public Task AddJobAsync(ProcessingJob job);
    }
}
