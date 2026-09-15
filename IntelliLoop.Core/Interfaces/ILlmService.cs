using IntelliLoop.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface ILlmService
    {
        Task<LectureAnalysis> AnalyzeLectureAsync(string transcript);
    }
}
