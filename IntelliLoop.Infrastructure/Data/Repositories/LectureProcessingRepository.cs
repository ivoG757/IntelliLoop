using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Infrastructure.Data.Repositories
{
    public class LectureProcessingRepository : ILectureProcessingRepository
    {
        private readonly IntelliLoopDbContext _context;
        public LectureProcessingRepository(IntelliLoopDbContext context)
        {
            _context = context; 
        }
        public async Task AddJobAsync(ProcessingJob job)
        {
            await _context.ProcessingJobs.AddAsync(job);
        }

        public async Task<ProcessingJob?> GetJobByIdAsync(Guid jobId, Guid userId)
        {
            return await _context.ProcessingJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == userId.ToString());
        }
        public async Task<ProcessingJob?> GetJobByIdAsync(Guid jobId)
        {
            return await _context.ProcessingJobs
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<IEnumerable<ProcessingJob>> GetJobsByUserIdAsync(Guid userId, int skip, int take)
        {
            return await _context.ProcessingJobs
                .AsNoTracking()
                .Include(j => j.Lecture)
                .Where(j => j.UserId == userId.ToString())
                .OrderByDescending(j => j.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Lecture?> GetLectureByJobIdAsync(Guid jobId, Guid userId)
        {
            return await _context.Lectures
                .AsNoTracking()
                .Include(l => l.Notes)
                .FirstOrDefaultAsync(l => l.ProcessingJob!.Id == jobId && l.UserId == userId.ToString());
        }
    }
}
