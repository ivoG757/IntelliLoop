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
        private readonly Channel<Guid> _jobChannel;
        private readonly ILectureProcessingRepository _lectureGenerationRepository;
        private readonly ILogger<LectureGenerationService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _uof;
        public LectureGenerationService(Channel<Guid> jobChannel,
            ILectureProcessingRepository lectureGenerationRepository,
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

        public async Task<Guid> QueueLectureGenerationAsync(IFormFile file, string userId)
        {

            var filePath = await _fileStorage.SaveAsync(file, userId);

            ProcessingJob job = new ProcessingJob
            {
                FilePath = filePath,
                Status = LectureGenerationStatus.Queued,
                CreatedAt = DateTime.UtcNow
            };

            await _lectureGenerationRepository.AddJobAsync(job);

            await _uof.SaveChangesAsync();

            await _jobChannel.Writer.WriteAsync(job.Id);

            return job.Id;
        }

        public async Task<Lecture> GetLectureByIdAsync(Guid jobId)
        {
            return new Lecture();
        }
    }
}
