using IntelliLoop.Core.Entities.Models;
namespace IntelliLoop.Core.Contracts.Services
{
    public interface ILectureService
    {
        public Task<Guid> QueueLectureGenerationAsync(Stream file, string fileName, Guid userId);
        public Task<ProcessingJob?> GetLectureJobByIdAsync(Guid jobId, Guid userId);
        public Task<Stream?> GetAudioByJobIdAsync(Guid jobId, Guid userId);
        public Task<Lecture?> GetLectureByJobIdAsync(Guid jobId, Guid userId);
    }
}
