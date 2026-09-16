using IntelliLoop.Core.Entities.Enums;
using Microsoft.AspNetCore.Http;
namespace IntelliLoop.Core.Interfaces
{
    public interface ILectureGenerationService
    {
        public Task<string> QueueLectureGenerationAsync(IFormFile file, string userId);
        public Task GetLectureByIdAsync(string jobId);
    }
}
