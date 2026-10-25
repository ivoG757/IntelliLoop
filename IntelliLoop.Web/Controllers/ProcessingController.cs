using IntelliLoop.Common.Constants;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Web.Mapping;
using IntelliLoop.Web.Models.Processing;
using Microsoft.AspNetCore.Mvc;

namespace IntelliLoop.Web.Controllers
{
    public class ProcessingController : BaseController
    {
        private ILectureService _lectureService { get; set; }
        public ProcessingController(ILectureService service)
        {
            _lectureService = service;
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
            if (file is null || file.Length == 0)
            {
                ModelState.AddModelError(nameof(file), "Please select a file.");
                return View();
            }

            string fileName = Path.GetFileName(file.FileName);

            if (string.IsNullOrWhiteSpace(fileName) ||
                !AudioConstants.AllowedAudioExtensions.Contains(
                    Path.GetExtension(fileName),
                    StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(file), "The file type is not supported.");
                return View();
            }

            if (file.Length > AudioConstants.MaxAudioFileSizeBytes)
            {
                ModelState.AddModelError(nameof(file),
                    $"The file must not exceed {AudioConstants.MaxAudioFileSizeBytes / (1024 * 1024)} MB.");
                return View();
            }

            await using var stream = file.OpenReadStream();

            var request = new UploadLectureRequestDto
            {
                Content = stream,
                UserId = GetUserId(),
                FileName = file.FileName ?? string.Empty
            };

            Guid jobId = await _lectureService.CreateJobAsync(request);

            return RedirectToAction(nameof(Index),
                new
                {
                    id = jobId

                });
        }

        [HttpGet]
        public async Task<IActionResult> ProcessingFailed([FromRoute] Guid id)
        {
            return View(); //TODO: ADD AN ACTUAL FUNCTIONALITY!
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var jobRequest = new JobRequestDto
            {
                UserId = GetUserId(),
                JobId = id
            };

            JobResponseDto? response = await _lectureService.GetJobAsync(jobRequest);

            if (response == null)
            {
                return NotFound();
            }

            if (response.Status == LectureGenerationStatus.Failed)
            {
                return RedirectToAction(nameof(ProcessingFailed), new { id = response.LectureId });
            }

            if (response.Status != LectureGenerationStatus.Completed)
            {
                return View(BuildProcessingViewModel(response));
            }

            return RedirectToAction(nameof(Index), new { id = response.LectureId });
        }

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
