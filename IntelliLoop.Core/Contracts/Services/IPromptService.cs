namespace IntelliLoop.Core.Contracts.Services
{
    public interface IPromptService
    {
        public Dictionary<string, List<string>> GetExtractionPrompts();
        public Dictionary<string, List<string>> GetFormattingPrompts();
    }
}
