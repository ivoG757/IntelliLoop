using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Models;
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
        public IActionResult Index([FromRoute] string id)
        {
            //TODO: Implement the logic to display the generated lecture based on the job ID.
            return View();
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
                ModelState.AddModelError("audioFile", "Please select an audio file.");
                return View();
            }


            Guid jobId = await _lectureGenerationService.QueueLectureGenerationAsync(file, userId);

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

            ProcessingJob job = await _lectureGenerationService.GetLectureJobByIdAsync(jobId);

            

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

            return RedirectToAction(nameof(Index), new { id = job.Id });
        }
    }
}

