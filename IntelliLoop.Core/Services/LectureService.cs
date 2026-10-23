using IntelliLoop.Common.Constants;
using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
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
        public readonly ILectureRepository _lectureRepository;
        private readonly ILogger<LectureService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _uof;
        public LectureService(Channel<Guid> jobChannel,
            ILectureProcessingRepository lectureGenerationRepository,
            ILectureRepository lectureRepository,
            ILogger<LectureService> logger,
            IFileStorage fileStorage,
            IUnitOfWork uof)
        {
            _lectureRepository = lectureRepository;
            _jobChannel = jobChannel;
            _lectureProcessingRepository = lectureGenerationRepository;
            _logger = logger;
            _fileStorage = fileStorage;
            _uof = uof;

        }


        public async Task<Guid> CreateJobAsync(UploadLectureRequestDto request)
        {
            Stream file = request.Content;
            string fileName = request.FileName;
            Guid userId = request.UserId;

            var filePath = string.Empty;

            try
            {
                filePath = await _fileStorage.SaveAsync(file, fileName, userId);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to save the uploaded file for user {UserId}.", userId);
                throw;
            }

            var job = new ProcessingJob
            {
                FilePath = filePath,
                Status = LectureGenerationStatus.Queued,
                CreatedAt = DateTime.UtcNow,
                UserId = userId.ToString()
            };

            try
            {
                await _lectureProcessingRepository.AddJobAsync(job);
                await _uof.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                await _fileStorage.Delete(filePath);

                _logger.LogError(ex, "Failed to add the processing job for user {UserId}.", userId);
                throw;
            }

            await _jobChannel.Writer.WriteAsync(job.Id);

            return job.Id;
        }

        public async Task<JobResponseDto?> GetJobAsync(JobRequestDto request)
        {
            var job = await _lectureProcessingRepository
                .GetJobByIdAsync(request.JobId, request.UserId);

            if (job is null)
            {
                _logger.LogWarning("Job {JobId} not found.",
                    request.JobId);
                return null;
            }

            return ToJobResponse(job);
        }

        public async Task<AudioResponseDto?> GetAudioAsync(LectureRequestDto request)
        {
            var job = await _lectureProcessingRepository.GetJobByLectureIdAsync(request.LectureId, request.UserId);

            if (job is null)
            {
                _logger.LogWarning("Lecture generation job not found.");
                return null;
            }

            var path = job.FilePath;

            if (string.IsNullOrWhiteSpace(path) || !_fileStorage.Exists(path))
            {
                _logger.LogWarning("Audio file for job {JobId} was not found.", 
                    job.Id);
                return null;
            }

            var audioStream = _fileStorage.GetStream(path);

            return new AudioResponseDto
            {
                Content = audioStream,
                ContentType = AudioConstants.WavContentType
            };
        }


        public async Task<LectureResponseDto?> GetLectureAsync(LectureRequestDto request)
        {
            var lecture = await _lectureRepository
                .GetByIdAsync(request.LectureId, request.UserId);

            if(lecture is null)
            {
                return null;
            }

            return new LectureResponseDto
            {
                Title = lecture.Title,
                Summary = lecture.Summary,
                Concepts = lecture.Concepts,
                Transcript = lecture.Transcript,
                Notes = lecture.Notes.Select(n => 
                new NoteResponseDto
                {
                    Title = n.Title,
                    Content = n.Content
                }).ToList()
            };
        }

        public async Task<ICollection<JobResponseDto>> GetJobsAsync(JobListRequestDto request)
        {
            var processingJobs = await _lectureProcessingRepository
                .GetJobsByUserIdAsync(request.UserId, request.Skip, request.Take);

            return processingJobs.Select(ToJobResponse).ToList();
        }

        public async Task<bool> DeleteAsync(LectureRequestDto request)
        {
            var isDeleted = await _lectureRepository.DeleteByIdAsync(request.LectureId, request.UserId);

            await _uof.SaveChangesAsync();

            return isDeleted;
        }

        private static JobResponseDto ToJobResponse(ProcessingJob job) => new()
        {
            JobId = job.Id,
            LectureId = job.LectureId,
            Title = job.Lecture?.Title ?? string.Empty,
            Status = job.Status
        };
    }
}
