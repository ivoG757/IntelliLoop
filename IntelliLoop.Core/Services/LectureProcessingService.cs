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

        public async Task ProcessLectureAsync(Guid jobId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing lecture generation job with ID: {JobId}", jobId);

            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId);

            if (job == null)
            {
                _logger.LogError("Lecture generation job with ID: {JobId} not found.", jobId);
                return;
            }

            job.StartedAt = DateTime.UtcNow;
            var transcript = string.Empty;
            job.Status = LectureGenerationStatus.Transcribing;

            await _uof.SaveChangesAsync();

            try
            {
                _logger.LogInformation("Transcribing lecture for generation job with ID: {JobId}", jobId);
                transcript = await _transcriptionService.TranscribeAudioAsync(job.FilePath, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed lecture transcription for generation job with ID: {JobId}", jobId);
                job.Status = LectureGenerationStatus.Failed;
                job.FailedAtStage = LectureGenerationStatus.Transcribing;
                await _uof.SaveChangesAsync();
                return;
            }
            job.Status = LectureGenerationStatus.Extracting;
            await _uof.SaveChangesAsync();

            LectureAnalysis summary = new();
            try
            {
                _logger.LogInformation("Extracting lecture generation job with ID: {JobId}", jobId);
                summary = await _llmService.AnalyzeLectureAsync(transcript, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed lecture extraction for generation job with ID: {JobiId}", jobId);
                job.Status = LectureGenerationStatus.Failed;
                job.FailedAtStage = LectureGenerationStatus.Extracting;
                await _uof.SaveChangesAsync();
                return;
            }

            var lecture = new Lecture
            {
                Title = summary.Title,
                Summary = summary.Summary,
                Concepts = summary.KeyConcepts,
                Notes = summary.Notes.Select(n => new NoteSection
                {
                    Title = n.Title,
                    Content = n.Content
                }).ToList(),
                Transcript = transcript,
                ProcessingJob = job,
                UserId = job.UserId
            };

            await _lectureRepository.AddAsync(lecture);

            job.Status = LectureGenerationStatus.Completed;

            job.CompletedAt = DateTime.UtcNow;

            try
            {
                await _uof.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save generated lecture for job {JobId}", jobId);

                job.Status = LectureGenerationStatus.Failed;
                job.FailedAtStage = LectureGenerationStatus.Extracting;
                job.CompletedAt = null;

                await _uof.SaveChangesAsync();
                return;
            }

            //testing - remove later
            Console.WriteLine($"{lecture.Title}\n{lecture.Summary}\n{summary.KeyConcepts}\n{summary.Notes}");

            _logger.LogInformation("Completed lecture generation job with ID: {JobId}", jobId);

        }
    }
}
