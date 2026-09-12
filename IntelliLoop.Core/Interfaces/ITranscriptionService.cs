using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface ITranscriptionService
    {
        public Task<string> TranscribeAudioAsync(string audioFilePath);
    }
}
