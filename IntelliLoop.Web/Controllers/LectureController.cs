using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.DTOs;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
using IntelliLoop.Web.Models.Lecture;
using Microsoft.AspNetCore.Mvc;

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

            if (lecture is null)
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
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            LectureResponseDto? lecture = await _lectureService.GetLectureAsync(BuildLectureRequest(id));

            if (lecture is null)
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

            var result = await _lectureService.DeleteAsync(request);
            if (result == LectureOperationResult.Failed)
            {
                return StatusCode(500);
            }
            if (result == LectureOperationResult.NotFound)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(ProcessingController.Upload), "Processing");
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

            var result = await _lectureService.UpdateAsync(request);

            if (result == LectureOperationResult.Failed)
            {
                return StatusCode(500);
            }
            if (result == LectureOperationResult.NotFound)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { id = model.Id });
        }
        private LectureRequestDto BuildLectureRequest(Guid lectureId) =>

            new LectureRequestDto
            {
                LectureId = lectureId,
                UserId = GetUserId()
            };
    }
}

