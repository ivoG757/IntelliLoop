using IntelliLoop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{

    public class TranscriptionController : Controller
    {
        private readonly TranscriptionService _transcriptionService;
        public TranscriptionController(TranscriptionService transcriptionService)
        {
            _transcriptionService = transcriptionService;
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
                var transcript = await _transcriptionService.TranscribeAudio(filePath);

                ViewBag.Transcript = transcript;

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
    }
}
