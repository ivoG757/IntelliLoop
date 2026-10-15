using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{
    public class LectureController : Controller
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
            var lecture = await _lectureService.GetLectureByJobIdAsync(id);

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
            var audioFile = await _lectureService.GetAudioByJobIdAsync(id);
            
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
            //string userId = User.Identity!.Name!;
            string userId = Guid.NewGuid().ToString();

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
        public async Task<IActionResult> Processing([FromRoute] string id)
        {

            var IdIsGuid = Guid.TryParse(id, out var jobId);

            if (!IdIsGuid)
            {
                return BadRequest(ModelState);
            }

            ProcessingJob job = await _lectureService.GetLectureJobByIdAsync(jobId);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Status != LectureGenerationStatus.Completed)
            {
                return View(new ProcessingJobViewModel
                {
                    Id = job.Id,
                    Status = job.Status,
                });
            }

            if (job.LectureId == null)
            {
                _logger.LogError("Job {JobId} is completed but has no associated lecture.", job.Id);

                return Problem("The lecture could not be found.");
            }

            return RedirectToAction(nameof(Index), new { id = job.Id });
        }
    }
}

