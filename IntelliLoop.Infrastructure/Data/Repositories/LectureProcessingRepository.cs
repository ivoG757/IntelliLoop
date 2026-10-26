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

        public void Delete(ProcessingJob job)
        {
            _context.ProcessingJobs.Remove(job);
        }

        public async Task<ProcessingJob?> GetJobByIdAsync(Guid jobId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.ProcessingJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == userIdString);
        }


        public async Task<ProcessingJob?> GetJobByIdAsync(Guid jobId)
        {

            return await _context.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<ProcessingJob?> GetJobByLectureIdAsync(Guid lectureId, Guid userId)
        {
            var userIdString = userId.ToString();
            return await _context.ProcessingJobs.FirstOrDefaultAsync(j => j.LectureId == lectureId && j.UserId == userIdString);
        }

        public async Task<ProcessingJob?> GetJobForDeletionByIdAsync(Guid jobId, Guid userId)
        {
            var userIdString = userId.ToString();

            return await _context.ProcessingJobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == userIdString);
        }

        public async Task<IEnumerable<ProcessingJob>> GetJobsByUserIdAsync(Guid userId, int skip, int take)
        {
            var userIdString = userId.ToString();

            return await _context.ProcessingJobs
                .AsNoTracking()
                .Include(j => j.Lecture)
                .Where(j => j.UserId == userIdString)
                .OrderByDescending(j => j.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
