using IntelliLoop.Core.Entities.Models;
namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface ILectureRepository
    {
        public Task AddAsync(Lecture lecture);
        public Task<Lecture?> GetByIdAsNoTrackingAsync(Guid lectureId, Guid userId);
        public Task<Lecture?> GetByIdAsync(Guid lectureId, Guid userId);
        Task<Lecture?> GetByIdForDeletionAsync(Guid lectureId, Guid userId);
        public void Delete(Lecture lecture);
    }
}
