using IntelliLoop.Core.Entities.Models;

namespace IntelliLoop.Web.Models
{
    public class SidebarViewModel
    {
        public string UserEmail { get; set; } = "";
        public List<SidebarItemViewModel> Lectures { get; set; } = new();
    }
}
