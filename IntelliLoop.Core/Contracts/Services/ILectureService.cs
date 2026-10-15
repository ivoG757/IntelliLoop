using IntelliLoop.Core.Entities.Models;
namespace IntelliLoop.Core.Contracts.Services
{
    public interface ILectureService
    {
        public Task<Guid> QueueLectureGenerationAsync(Stream file, string fileName, string userId);
        public Task<ProcessingJob?> GetLectureJobByIdAsync(Guid jobId);
        public Task<Stream?> GetAudioByJobIdAsync(Guid jobId);
        public Task<Lecture?> GetLectureByJobIdAsync(Guid jobId);
    }
}
