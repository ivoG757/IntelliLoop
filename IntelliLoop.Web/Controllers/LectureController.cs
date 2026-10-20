using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
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
        public async Task<IActionResult> Index(Guid id)
        {
            var userId = GetUserId();

            var lecture = await _lectureService.GetLectureByJobIdAsync(id, userId);

            var lectureViewModel = new LectureViewModel
            {
                Title = lecture?.Title ?? "Lecture Not Found",
                Summary = lecture?.Summary ?? "No summary available.",
                KeyConcepts = lecture?.KeyConcepts ?? new List<string>(),
                Notes = lecture?.Notes.Select(n => new NoteViewModel
                {
                    Title = n.Title,
                    Content = n.Content
                }).ToList() ?? new List<NoteViewModel>(),

                Transcript = lecture?.Transcript ?? "No transcript available.",
                JobId = id,
            };

            return View(lectureViewModel);
        }
        public async Task<IActionResult> Audio(Guid id)
        {
            var userId = GetUserId();
            var audioFile = await _lectureService.GetAudioByJobIdAsync(id, userId);

            if (audioFile == null)
            {
                return NotFound();
            }

            return File(audioFile, "audio/wav", enableRangeProcessing: true);
        }


        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var userId = GetUserId();

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select an audio file.");
                return View();
            }

            Guid jobId = await _lectureService.QueueLectureGenerationAsync(file.OpenReadStream(), file.FileName, userId);

            return RedirectToAction(nameof(Processing),
                new
                {
                    Id = jobId
                });
        }

        [HttpGet]
        public async Task<IActionResult> Processing([FromRoute] Guid id)
        {
            var userId = GetUserId();

            ProcessingJob? job = await _lectureService.GetLectureJobByIdAsync(id, userId);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Status == LectureGenerationStatus.Failed)
            {
                return View("Error"); //TODO: Add an actual Error page with retry
            }

            if(job.Status != LectureGenerationStatus.Completed)
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

                return View(new ProcessingJobViewModel
                {
                    Id = job.Id,
                    Title = StatusManager.Map(job.Status),
                    Steps = steps
                });
            }


            return RedirectToAction(nameof(Index), new { id = job.Id });
        }
    }
}

