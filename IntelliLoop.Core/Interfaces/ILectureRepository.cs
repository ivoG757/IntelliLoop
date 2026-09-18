using IntelliLoop.Core.Entities;

namespace IntelliLoop.Core.Interfaces
{
    public interface ILectureRepository
    {
        public Task AddLectureAsync(Lecture job);
    }
}
