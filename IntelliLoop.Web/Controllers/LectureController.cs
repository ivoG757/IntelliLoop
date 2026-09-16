using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Services.Background;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{

    public class LectureController : Controller
    {
        private readonly ITranscriptionService _transcriptionService;
        private readonly ILlmService _lectureService;
        private readonly BackgroundTaskQueue _queue;
        private readonly ILectureGenerationService _lectureGenerationService;
        public LectureController(ITranscriptionService transcriptionService,
            ILlmService lectureService,
            BackgroundTaskQueue queue,
            ILectureGenerationService lectureGenerationService)
        {
            _transcriptionService = transcriptionService;
            _lectureService = lectureService;
            _queue = queue;
            _lectureGenerationService = lectureGenerationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("audioFile", "Please select an audio file.");
                return View();
            }
            var userId = User.Identity!.Name!;

            var jobId = await _lectureGenerationService.QueueLectureGenerationAsync(file, userId);

            return RedirectToAction(nameof(Lecture), new { jobId = jobId });
        }

        [HttpGet]
        public async Task<IActionResult> Lecture(string id)
        {
            await _lectureGenerationService.GetLectureByIdAsync(id);

            return View(nameof(Index));
        }
    }
}
