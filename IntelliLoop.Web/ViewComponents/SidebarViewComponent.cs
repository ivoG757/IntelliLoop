using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Web.Mapping;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static IntelliLoop.Web.Common.ApplicationConstants;
using static IntelliLoop.Common.Constants.AccountConstants;
using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Web.Models.Sidebar;

namespace IntelliLoop.Web.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly ILectureService _lectureService;

    public SidebarViewComponent(ILectureService lectureService)
    {
        _lectureService = lectureService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Content(string.Empty);
        }

        var userIdValue = UserClaimsPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Content(string.Empty);
        }

        var request = new JobListRequestDto()
        {
            UserId = userId,
            Take = Take,
            Skip = 0
        };

        var jobs = await _lectureService.GetJobsAsync(request);

        var sidebarItems = jobs.Select(dto =>
        new SidebarItemViewModel
        {
            JobId = dto.JobId,
            LectureId = dto.LectureId,
            Title = string.IsNullOrWhiteSpace(dto.Title)
            ? StatusManager.Map(dto.Status)
            : dto.Title,
            Status = dto.Status
        }).ToList();

        SidebarViewModel model = new SidebarViewModel
        {
            UserEmail = UserClaimsPrincipal
            .FindFirstValue(ClaimTypes.Email) ?? DefaultUser,

            Lectures = sidebarItems
        };

        return View(model);
    }
}