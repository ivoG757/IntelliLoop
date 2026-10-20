using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Web.Mapping;
using IntelliLoop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        var jobs = await _lectureService.GetJobsByUserIdAsync(userId);

        var sidebarItems = jobs.Select(dto =>
        new SidebarItemViewModel
        {
            JobId = dto.JobId,
            Title = string.IsNullOrWhiteSpace(dto.Title)
            ? StatusManager.Map(dto.Status)
            : dto.Title,
            Status = dto.Status
        }).ToList();

        SidebarViewModel model = new SidebarViewModel
        {
            UserEmail = UserClaimsPrincipal
            .FindFirstValue(ClaimTypes.Email) ?? "User",

            Lectures = sidebarItems
        };

        return View(model);
    }
}