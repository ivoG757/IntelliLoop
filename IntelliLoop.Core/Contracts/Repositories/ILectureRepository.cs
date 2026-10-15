using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface ILectureRepository
    {
        public Task AddLectureAsync(Lecture lecture);
    }
}
