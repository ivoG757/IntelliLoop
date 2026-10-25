using IntelliLoop.Core.DTOs;

namespace IntelliLoop.Core.Contracts.Services
{
    public interface ILlmService
    {
        Task<LectureAnalysis> AnalyzeLectureAsync(string transcript, CancellationToken token);
    }
}
