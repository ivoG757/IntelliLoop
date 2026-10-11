using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Repositories;
using System.Threading.Channels;

namespace IntelliLoop.Web.Services
{
    public class LectureService : ILectureService
    {
        private readonly Channel<Guid> _jobChannel;
        private readonly ILectureProcessingRepository _lectureProcessingRepository;
        private readonly ILogger<LectureService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _uof;
        public LectureService(Channel<Guid> jobChannel,
            ILectureProcessingRepository lectureGenerationRepository,
            ILogger<LectureService> logger,
            IFileStorage fileStorage, ILectureRepository lectureRepository,
            IUnitOfWork uof)
        {
            _jobChannel = jobChannel;
            _lectureProcessingRepository = lectureGenerationRepository;
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

            await _lectureProcessingRepository.AddJobAsync(job);

            await _uof.SaveChangesAsync();

            await _jobChannel.Writer.WriteAsync(job.Id);

            return job.Id;
        }

        public async Task<Stream?> GetAudioByJobIdAsync(Guid jobId)
        {
            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId);

            if (job == null)
            {
                _logger.LogWarning($"Lecture generation job with ID: {jobId} not found.");
                return null;
            }

            if (job.Status != LectureGenerationStatus.Completed)
            {
                _logger.LogWarning($"Lecture generation job with ID: {jobId} is not completed yet.");

                return null;
            }

            var path = await _lectureProcessingRepository.GetAudioFilePathByJobIdAsync(job.Id);

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                _logger.LogWarning("Audio file for job {JobId} was not found.", jobId);
                return null;
            }

            var audioStream = _fileStorage.GetStream(path);
          
            return audioStream;
        }

        public async Task<ProcessingJob> GetLectureJobByIdAsync(Guid jobId)
        {
            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogWarning($"Lecture generation job with ID: {jobId} not found.");
                return null;
            }
            return job;
        }

        public async Task<Lecture> GetLectureByJobIdAsync(Guid jobId)
        {
            var lecture = await _lectureProcessingRepository.GetLectureByJobIdAsync(jobId);

            return lecture;
        }
    }
}
