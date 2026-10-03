using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using Microsoft.AspNetCore.Http;
namespace IntelliLoop.Core.Interfaces
{
    public interface ILectureGenerationService
    {
        public Task<Guid> QueueLectureGenerationAsync(IFormFile file, string userId);
        public Task<ProcessingJob> GetLectureJobByIdAsync(Guid jobId);
    }
}
