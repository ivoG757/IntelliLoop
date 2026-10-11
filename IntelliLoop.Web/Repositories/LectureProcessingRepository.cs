using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Web.Repositories
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

        public async Task<ProcessingJob?> GetJobByIdAsync(Guid jobId)
        {
            return await _context.ProcessingJobs.FindAsync(jobId);
        }

        public async Task<Lecture?> GetLectureByJobIdAsync(Guid jobId)
        {
            return await _context.Lectures
                .Include(l => l.Notes)
                .FirstOrDefaultAsync(l => l.ProcessingJob!.Id == jobId);
        }
        
        public async Task<string> GetAudioFilePathByJobIdAsync(Guid jobId)
        {
            var job = await _context.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == jobId);
            return job?.FilePath ?? string.Empty;
        }
    }
}
