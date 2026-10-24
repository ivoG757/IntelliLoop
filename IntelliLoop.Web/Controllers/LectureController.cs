using IntelliLoop.Common.Constants;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
using IntelliLoop.Core.Entities.Enums;
using IntelliLoop.Web.Mapping;
using IntelliLoop.Web.Models.Lecture;
using IntelliLoop.Web.Models.Processing;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IntelliLoop.Web.Controllers
{
    public class LectureController : BaseController
    {
        private readonly ILectureService _lectureService;

        public LectureController(ILectureService lectureService)
        {
            _lectureService = lectureService;
        }
        public async Task<IActionResult> Index([FromRoute] Guid id)
        {
            LectureResponseDto? lecture = await _lectureService
                .GetLectureAsync(BuildLectureRequest(id));

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
                Id = id
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Audio(Guid id)
        {
            AudioResponseDto? audio = await _lectureService.GetAudioAsync(BuildLectureRequest(id));

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
                UserId = GetUserId(),
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
                return View("Error"); //TODO: Add an actual Error page with retry
            }

            if(response.Status != LectureGenerationStatus.Completed)
            {
                return View(BuildProcessingViewModel(response));
            }

            return RedirectToAction(nameof(Index), new { id = response.LectureId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            LectureResponseDto? lecture = await _lectureService.GetLectureAsync(BuildLectureRequest(id));

            if(lecture is null)
            {
                return NotFound();
            }

            var model = new DeleteViewModel
            {
                Id = id,
                LectureTitle = lecture.Title
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmedDelete([FromRoute] Guid id)
        {
            var request = BuildLectureRequest(id);

            var isDeleted = await _lectureService.DeleteAsync(request);

            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Upload));
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            LectureResponseDto? lecture = await _lectureService
                .GetLectureAsync(BuildLectureRequest(id));

            if (lecture is null)
            {
                return NotFound();
            }

            var viewModel = new EditLectureViewModel
            {
                Title = lecture.Title,
                Summary = lecture.Summary,
                KeyConcepts = lecture.Concepts.ToList(),
                Notes = lecture.Notes.Select(n => new EditNoteViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content
                }).ToList(),
                Id = id
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditLectureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new UpdateLectureRequestDto
            {
                Id = model.Id,
                Title = model.Title,
                UserId = GetUserId(),
                Summary = model.Summary,
                Concepts = model.KeyConcepts,
                Notes = model.Notes.Select(n => new UpdateNotesRequestDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content
                }).ToList()
            };

            var isSaved = await _lectureService.UpdateAsync(request);
            
            if(!isSaved)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Upload));
        }

        private LectureRequestDto BuildLectureRequest(Guid lectureId) =>

            new LectureRequestDto
            {
                LectureId = lectureId,
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

