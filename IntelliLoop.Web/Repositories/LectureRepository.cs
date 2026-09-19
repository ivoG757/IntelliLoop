using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Data;

namespace IntelliLoop.Web.Repositories
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
