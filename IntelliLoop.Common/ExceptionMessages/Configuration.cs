using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Common.ExceptionMessages
{
    public static class Configuration
    {
        public const string BasePathNotConfigured = "Base path not configured";
        public const string ConnectionStringNotFound = "Connection string 'DefaultConnection' not found.";
        public const string WhisperModelPathNotConfigured = "Whisper model path not configured";
        public const string FileStoragePathNotConfigured = "File storage path not configured";
        public const string PromptsPathNotConfigured = "Prompts path not configured";
    }
}
