using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace IntelliLoop.Core.Services
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
            IFileStorage fileStorage, 
            ILectureRepository lectureRepository,
            IUnitOfWork uof)
        {
            _jobChannel = jobChannel;
            _lectureProcessingRepository = lectureGenerationRepository;
            _logger = logger;
            _fileStorage = fileStorage;
            _uof = uof;

        }


        public async Task<Guid> QueueLectureGenerationAsync(Stream file, string fileName, Guid userId)
        {
            var filePath = string.Empty;

            try
            {
                filePath = await _fileStorage.SaveAsync(file, fileName, userId);

            }
            catch
            {
                _logger.LogError("Failed to save the uploaded file for user {UserId}.", userId);
                throw;
            }

            ProcessingJob job = new ProcessingJob
            {
                FilePath = filePath,
                Status = LectureGenerationStatus.Queued,
                CreatedAt = DateTime.UtcNow,
                UserId = userId.ToString()
            };

            try
            {
                await _lectureProcessingRepository.AddJobAsync(job);
            }
            catch
            {
                await _fileStorage.Delete(filePath);

                _logger.LogError("Failed to add the processing job for user {UserId}.", userId);
                throw;
            }

            await _uof.SaveChangesAsync();

            await _jobChannel.Writer.WriteAsync(job.Id);

            return job.Id;
        }

        public async Task<Stream?> GetAudioByJobIdAsync(Guid jobId, Guid userId)
        {
            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId, userId);

            if (job == null)
            {
                _logger.LogWarning("Lecture generation job with ID: {jobId} not found.", jobId);
                return null;
            }

            if (job.Status != LectureGenerationStatus.Completed)
            {
                _logger.LogWarning("Lecture generation job with ID: {jobId} is not completed yet.", jobId);

                return null;
            }

            var path = job.FilePath;

            if (string.IsNullOrWhiteSpace(path) || !_fileStorage.Exists(path))
            {
                _logger.LogWarning("Audio file for job {JobId} was not found.", jobId);
                return null;
            }

            var audioStream = _fileStorage.GetStream(path);
          
            return audioStream;
        }

        public async Task<ProcessingJob?> GetLectureJobByIdAsync(Guid jobId, Guid userId)
        {
            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId, userId);

            if (job == null)
            {
                _logger.LogWarning("Lecture generation job with ID: {JobId} not found.", jobId);
                return null;
            }

            return job;
        }

        public async Task<Lecture?> GetLectureByJobIdAsync(Guid jobId, Guid userId)
        {
            var lecture = await _lectureProcessingRepository.GetLectureByJobIdAsync(jobId, userId);

            return lecture;
        }
    }
}
