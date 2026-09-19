using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{

    public class LectureController : Controller
    {
        private readonly ILectureGenerationService _lectureGenerationService;
        private readonly ILogger<LectureController> _logger;
        public LectureController(ILectureGenerationService lectureGenerationService, 
            ILogger<LectureController> logger)
        {
            _lectureGenerationService = lectureGenerationService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string userId = User.Identity!.Name!;

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("audioFile", "Please select an audio file.");
                return View();
            }


            Guid jobId = await _lectureGenerationService.QueueLectureGenerationAsync(file, userId);

            return RedirectToAction(nameof(Lecture),
                new
                {
                    jobId = jobId
                });
        }

        [HttpGet]
        public async Task<IActionResult> Lecture(string id)
        {
            var IdIsGuid = Guid.TryParse(id, out var lectureId);

            if(!IdIsGuid)
            {
                ModelState.AddModelError("Id", "Please provide lecture id");
            }

            Lecture job = await _lectureGenerationService.GetLectureByIdAsync(lectureId);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
    }
}
