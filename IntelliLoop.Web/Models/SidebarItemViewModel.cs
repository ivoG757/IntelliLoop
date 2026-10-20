using IntelliLoop.Core.Entities.Enums;

namespace IntelliLoop.Web.Models
{
    public class SidebarItemViewModel
    {
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public LectureGenerationStatus Status { get; set; }
    }
}
