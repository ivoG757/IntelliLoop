using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Services.Background;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{

    public class TranscriptionController : Controller
    {
        private readonly ITranscriptionService _transcriptionService;
        private readonly ILlmService _lectureService;
        private readonly BackgroundTaskQueue _queue;
        public TranscriptionController(ITranscriptionService transcriptionService,
            ILlmService lectureService,
            BackgroundTaskQueue queue)
        {
            _transcriptionService = transcriptionService;
            _lectureService = lectureService;
            _queue = queue;
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

            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(file.FileName)); 
            // Temporary file path, replacing database for now

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Call the transcription service
                var transcript = await _transcriptionService.TranscribeAudioAsync(filePath);

                return View(nameof(Index));

            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while processing the file: {ex.Message}");
                return View(nameof(Index));
            }

            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Summarize(string transcript)
        {

            await _queue.QueueAsync(async () => 
            {
                var lecture = await _lectureService.AnalyzeLectureAsync(transcript);
                Console.WriteLine("AI RESULT:");

                var output = System.Text.Json.JsonSerializer.Serialize(lecture, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                Console.WriteLine(output);
                ViewBag.Lecture = output;
            });

            return View(nameof(Index));
        }
    }
}
