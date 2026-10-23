using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface ILectureRepository
    {
        public Task AddAsync(Lecture lecture);
        public Task<Lecture?> GetByIdAsync(Guid lectureId, Guid userId);
        public Task<bool> DeleteByIdAsync(Guid lectureId, Guid userId);

    }
}
