using IntelliLoop.Common.Constants;
using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using Microsoft.Extensions.Logging;

namespace IntelliLoop.Core.Services
{
    public class LectureProcessingService : ILectureProcessingService
    {
        private readonly ILlmService _llmService;
        private readonly ITranscriptionService _transcriptionService;
        private readonly ILogger<LectureProcessingService> _logger;
        private readonly ILectureProcessingRepository _lectureProcessingRepository;
        private readonly ILectureRepository _lectureRepository;
        private readonly IUnitOfWork _uof;

        public LectureProcessingService(ILlmService llmService, 
            ITranscriptionService transcriptionService,
            ILogger<LectureProcessingService> logger,
            ILectureProcessingRepository lectureProcessingRepository,
            ILectureRepository lectureRepository,
            IUnitOfWork uof)
        {
            _llmService = llmService;
            _transcriptionService = transcriptionService;
            _logger = logger;
            _lectureProcessingRepository = lectureProcessingRepository;
            _lectureRepository = lectureRepository;
            _uof = uof;
        }
        //TODO: Add error handling and logging for each step of the process.
        //Think of a way to represent the error in the ProcessingJob entity so that the user can see what went wrong if the job fails.
        //Also, consider adding a retry mechanism for transient errors.

        public async Task ProcessLectureAsync(Guid jobId)
        {
            _logger.LogInformation($"Processing lecture generation job with ID: {jobId}");

            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId);

            if (job == null)
            {
                _logger.LogError($"Lecture generation job with ID: {jobId} not found.");
                return;
            }

            job.Status = LectureGenerationStatus.Transcribing;

            job.StartedAt = DateTime.UtcNow;

            await _uof.SaveChangesAsync();

            _logger.LogInformation($"Transcribing lecture generation job with ID: {jobId}");
            string transcript = await _transcriptionService.TranscribeAudioAsync(job.FilePath);

            job.Status = LectureGenerationStatus.Extracting;

            await _uof.SaveChangesAsync();

            LectureAnalysis summary = await _llmService.AnalyzeLectureAsync(transcript);

            _logger.LogInformation($"Extracting lecture generation job with ID: {jobId}");

            var lecture = new Lecture
            {
                Title = summary.Title,
                Summary = summary.Summary,
                Concepts = summary.KeyConcepts,
                Notes = summary.Notes,
                Transcript = transcript,
                ProcessingJob = job,
                UserId = job.UserId
            };

            await _lectureRepository.AddLectureAsync(lecture);

            job.Status = LectureGenerationStatus.Completed;

            job.CompletedAt = DateTime.UtcNow;

            Console.WriteLine("DEBUG 1: Before final SaveChanges");
            try
            {
                await _uof.SaveChangesAsync();
                Console.WriteLine("DEBUG 2: Final SaveChanges completed");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"DEBUG ERROR: {ex}");
                throw;
            }
            Console.WriteLine("DEBUG 3: After final SaveChanges");

            //testing - remove later
            Console.WriteLine($"{lecture.Title}\n{lecture.Summary}\n{summary.KeyConcepts}\n{summary.Notes}");

            _logger.LogInformation($"Completed lecture generation job with ID: {jobId}");

        }

        //TODO: validate the uploaded audio file before saving it to the storage.
        private static void ValidateAudioUpload(Stream file, string fileName)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (!file.CanRead)
            {
                throw new ArgumentException(
                    "The uploaded file cannot be read.", nameof(file));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(
                    "A file name is required.", nameof(fileName));
            }

            var extension = Path.GetExtension(fileName);

            if (!AudioConstants.AllowedAudioExtensions.Contains(extension))
            {
                throw new InvalidDataException(
                    "Unsupported file type. Upload an MP3, WAV, M4A, OGG, or WebM file.");
            }

            if (!file.CanSeek)
            {
                throw new InvalidOperationException(
                    "The upload stream must support seeking to validate its size.");
            }

            if (file.Length == 0)
            {
                throw new InvalidDataException("The uploaded file is empty.");
            }

            if (file.Length > AudioConstants.MaxAudioFileSizeBytes)
            {
                throw new InvalidDataException(
                    "The audio file cannot exceed 100 MB.");
            }

            file.Position = 0;
        }
    }
}
