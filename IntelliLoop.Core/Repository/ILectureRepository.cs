using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Core.Repository
{
    public interface ILectureRepository
    {
        public Task AddLectureAsync(Lecture lecture);
    }
}
