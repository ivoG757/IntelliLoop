using IntelliLoop.Core.DTOs;
using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Repository;

namespace IntelliLoop.Web.Services
{
    public class LectureProcessingService
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
            _uof = uof;
            _lectureRepository = lectureRepository;
        }

        public async Task ProcessLectureAsync(Guid jobId)
        {
            _logger.LogInformation($"Processing lecture generation job with ID: {jobId}");

            var job = await _lectureProcessingRepository.GetJobByIdAsync(jobId);

            job.Status = LectureGenerationStatus.Processing;

            await _uof.SaveChangesAsync();


            var transcript = await _transcriptionService.TranscribeAudioAsync(job.FilePath);

            LectureAnalysis summary = await _llmService.AnalyzeLectureAsync(transcript);

            //TODO: add the transcript to the lecture object
            //TODO: add relationship between JobProcessing and Lecture entities
            var lectureAnalysis = new Lecture
            {
                Title = summary.Title,
                Summary = summary.Summary,
                KeyConcepts = summary.KeyConcepts,
                Notes = summary.Notes
            };


            await _lectureRepository.AddLectureAsync(lectureAnalysis);
            job.Status = LectureGenerationStatus.Completed;

            await _uof.SaveChangesAsync();

            _logger.LogInformation($"Completed lecture generation job with ID: {jobId}");

        }
    }
}
