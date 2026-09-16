using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Repository;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace IntelliLoop.Web.Services
{
    public class LectureGenerationService : ILectureGenerationService
    {
        private readonly Channel<LectureGenerationJob> _jobChannel;
        private readonly ILectureGenerationRepository _lectureGenerationRepository;
        private readonly ILogger<LectureGenerationService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _uof;
        public LectureGenerationService(Channel<LectureGenerationJob> jobChannel,
            ILectureGenerationRepository lectureGenerationRepository,
            ILogger<LectureGenerationService> logger,
            IFileStorage fileStorage,
            IUnitOfWork uof)
        {
            _jobChannel = jobChannel;
            _lectureGenerationRepository = lectureGenerationRepository;
            _logger = logger;
            _fileStorage = fileStorage;
            _uof = uof;

        }

        public async Task<string> QueueLectureGenerationAsync(IFormFile file, string userId)
        {

            var filePath = await _fileStorage.SaveAsync(file, userId);

            var job = new ProcessingJob
            {
                FilePath = filePath,
                Status = LectureGenerationStatus.Queued,
                CreatedAt = DateTime.UtcNow
            };

            await _lectureGenerationRepository.AddJobAsync(job);

            await _uof.SaveChangesAsync();

            var generationJob = new LectureGenerationJob
            {
                Id = job.Id.ToString(),
                FilePath = job.FilePath,
                Status = job.Status
            };

            await _jobChannel.Writer.WriteAsync(generationJob);

            return generationJob.Id;
        }

        public async Task GetLectureByIdAsync(string jobId)
        {
            var job = await _lectureGenerationRepository.GetJobByIdAsync(jobId);
            // Process the job...
        }
    }
}
