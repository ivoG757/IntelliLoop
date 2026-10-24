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

        public void Delete(Lecture lecture)
        {
            _context.Lectures.Remove(lecture);

            if (lecture.ProcessingJob is not null)
            {
                _context.ProcessingJobs.Remove(lecture.ProcessingJob);
            }
        }

        public async Task<Lecture?> GetByIdAsNoTrackingAsync(Guid lectureId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.Lectures
                .Include(l => l.Notes)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == lectureId && l.UserId == userIdString);
        }

        public async Task<Lecture?> GetByIdAsync(Guid lectureId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.Lectures
                .Include(l => l.Notes)
                .FirstOrDefaultAsync(l => l.Id == lectureId && l.UserId == userIdString);
        }
        public async Task<Lecture?> GetByIdForDeletionAsync(Guid lectureId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.Lectures
                .Include(l => l.ProcessingJob)
                .FirstOrDefaultAsync(l => l.Id == lectureId && l.UserId == userIdString);
        }
    }
}
