using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Infrastructure.Data.Repositories
{
    public class LectureRepository : ILectureRepository
    {
        private readonly IntelliLoopDbContext _context;
        public LectureRepository(IntelliLoopDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Lecture lecture)
        {
            await _context.Lectures.AddAsync(lecture);
        }

        public async Task<bool> DeleteByIdAsync(Guid lectureId, Guid userId)
        {
            var userIdString = userId.ToString();

            var lecture = await _context.Lectures
                .Include(l => l.ProcessingJob)
                .FirstOrDefaultAsync(l => l.Id == lectureId && l.UserId == userIdString);

            if (lecture is null)
            {
                return false;
            }

            if (lecture.ProcessingJob is not null)
            {
                _context.ProcessingJobs.Remove(lecture.ProcessingJob);
            }
            _context.Lectures.Remove(lecture);

            return true;
        }

        public async Task<Lecture?> GetByIdAsync(Guid LectureId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.Lectures
                .AsNoTracking()
                .Include(l => l.Notes)
                .FirstOrDefaultAsync(l => l.Id == LectureId && l.UserId == userIdString);
        }
    }
}
