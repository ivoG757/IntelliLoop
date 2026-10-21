namespace IntelliLoop.Web.Models
{
    public class ProcessingJobViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; init; } = "";
        public IReadOnlyList<ProcessingStepViewModel> Steps { get; init; } = [];
    }
}
