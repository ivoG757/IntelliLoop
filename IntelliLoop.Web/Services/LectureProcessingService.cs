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

        public async Task ProcessLectureAsync(string lectureId)
        {
            _logger.LogInformation($"Processing lecture generation job with ID: {lectureId}");

            var lecture = await _lectureProcessingRepository.GetJobByIdAsync(lectureId);

            lecture.Status = LectureGenerationStatus.Processing;

            await _uof.SaveChangesAsync();


            var transcriptTask = await _transcriptionService.TranscribeAudioAsync(lectureId);

            var summary = await _llmService.AnalyzeLectureAsync(transcriptTask);

            var lectureAnalysis = new Lecture
            {
                Title = summary.Title,
                Summary = summary.Summary,
                KeyConcepts = summary.KeyConcepts,
                Notes = summary.Notes
            };

            await _lectureRepository.AddLectureAsync(lectureAnalysis);

            await _uof.SaveChangesAsync();

            _logger.LogInformation($"Completed lecture generation job with ID: {lectureId}");

        }
    }
}
