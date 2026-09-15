using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface IPromptService
    {
        public Dictionary<string, List<string>> GetExtractionPrompts();
        public Dictionary<string, List<string>> GetFormattingPrompts();
    }
}
