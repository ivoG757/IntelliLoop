using IntelliLoop.Common.Constants;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Web.Mapping;
using IntelliLoop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{
    public class LectureController : BaseController
    {
        private readonly ILectureService _lectureService;
        private readonly ILogger<LectureController> _logger;

        public LectureController(ILectureService lectureService,
            ILogger<LectureController> logger)
        {
            _lectureService = lectureService;
            _logger = logger;
        }
        public async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var userId = GetUserId();

            LectureResponseDto? lecture = await _lectureService
                .GetLectureAsync(BuildJobRequest(id));

            if(lecture is null)
            {
                return NotFound();
            }

            var viewModel = new LectureViewModel
            {
                Title = lecture.Title,
                Summary = lecture.Summary,
                KeyConcepts = lecture.Concepts ?? new List<string>(),
                Notes = lecture.Notes.Select(n => new NoteViewModel
                {
                    Title = n.Title,
                    Content = n.Content
                }).ToList(),
                Transcript = lecture.Transcript ?? "No transcript available.",
                JobId = id
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Audio(Guid id)
        {
            AudioResponseDto? audio = await _lectureService.GetAudioAsync(BuildJobRequest(id));

            if (audio is null)
            {
                return NotFound();
            }

            return File(audio.Content, audio.ContentType, enableRangeProcessing: true);
        }


        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(AudioConstants.MaxAudioFileSizeBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = AudioConstants.MaxAudioFileSizeBytes)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var userId = GetUserId();

            if (file is null)
            {
                throw new ArgumentNullException();
            }

            string fileName = Path.GetFileName(file.FileName ?? string.Empty);
            if (string.IsNullOrWhiteSpace(fileName) ||
                !AudioConstants.AllowedAudioExtensions.Contains(Path.GetExtension(fileName)))
            {
                ModelState.AddModelError(nameof(file), "The file type is not supported.");
                return View();
            }

            if (file.Length > AudioConstants.MaxAudioFileSizeBytes)
            {
                ModelState.AddModelError("file",
                    $"Unsupported file type. Allowed: " +
                    $"{string.Join(", ", AudioConstants.AllowedAudioExtensions)}");
                return View();
            }

            await using var stream = file.OpenReadStream();

            var request = new UploadLectureRequestDto
            {
                Content = stream,
                UserId = userId,
                FileName = file.FileName ?? string.Empty
            };

            Guid jobId = await _lectureService.CreateJobAsync(request);

            return RedirectToAction(nameof(Processing),
                new
                {
                    id = jobId

                });
        }

        [HttpGet]
        public async Task<IActionResult> Processing([FromRoute] Guid id)
        {
            var userId = GetUserId();

            JobResponseDto? response = await _lectureService.GetJobAsync(BuildJobRequest(id));

            if (response == null)
            {
                return NotFound();
            }

            if (response.Status == LectureGenerationStatus.Failed)
            {
                return View("Error"); //TODO: Add an actual Error page with retry
            }

            if(response.Status != LectureGenerationStatus.Completed)
            {
                return View(BuildProcessingViewModel(response));
            }

            return RedirectToAction(nameof(Index), new { id = response.JobId });
        }
        private JobRequestDto BuildJobRequest(Guid jobId) =>
            new JobRequestDto
        {
            JobId = jobId,
            UserId = GetUserId()
        };

        private static ProcessingJobViewModel BuildProcessingViewModel(JobResponseDto job)
        {
            var stepNames = StatusManager.GetSteps(job.Status);
            int currentStep = (int)job.Status;

            var steps = new List<ProcessingStepViewModel>();

            for (int i = 0; i < stepNames.Count; i++)
            {
                steps.Add(new ProcessingStepViewModel
                {
                    Name = stepNames[i],
                    IsDone = i < currentStep,
                    IsCurrent = i == currentStep
                });
            }

            return new ProcessingJobViewModel
            {
                Id = job.JobId,
                Title = StatusManager.Map(job.Status),
                Steps = steps
            };
        }
    }
}

