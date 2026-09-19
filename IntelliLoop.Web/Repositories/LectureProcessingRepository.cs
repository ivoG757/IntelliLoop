using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Data;

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
    }
}
