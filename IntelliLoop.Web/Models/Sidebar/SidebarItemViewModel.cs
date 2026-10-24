using IntelliLoop.Core.Entities.Enums;

namespace IntelliLoop.Web.Models.Sidebar
{
    public class SidebarItemViewModel
    {
        public Guid? LectureId { get; set; }
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public LectureGenerationStatus Status { get; set; }
    }
}
