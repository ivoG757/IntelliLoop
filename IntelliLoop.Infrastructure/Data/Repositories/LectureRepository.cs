using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Infrastructure.Data.Repositories
{
    public class LectureRepository : ILectureRepository
    {
        private readonly IntelliLoopDbContext _context;
        public LectureRepository(IntelliLoopDbContext context)
        {
            _context = context;
        }

        public async Task AddLectureAsync(Lecture lecture)
        {
            await _context.Lectures.AddAsync(lecture);
        }
    }
}
