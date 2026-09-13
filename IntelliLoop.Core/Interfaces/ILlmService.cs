using IntelliLoop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface ILlmService
    {
        Task<Lecture> AnalyzeLectureAsync(string transcript);
    }
}
