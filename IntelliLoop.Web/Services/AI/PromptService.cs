using IntelliLoop.Core.Interfaces;

namespace IntelliLoop.Web.Services.AI
{
    public class PromptService : IPromptService
    {
        private readonly ILogger<PromptService> _logger;
        private readonly string _promptsPath;

        public PromptService(IConfiguration configuration, ILogger<PromptService> logger, IHostEnvironment environment)
        {
            _logger = logger;

            var promptPath = configuration["Prompts:Path"] ?? throw new InvalidOperationException("Prompts:Path configuration is missing.");

            _promptsPath = Path.Combine(environment.ContentRootPath, promptPath);
        }

        public Dictionary<string, List<string>> GetExtractionPrompts()
        {
            var folderName = "Extraction";

            ValidatePromptFolder(folderName);

            return GetPromptsFromFolder(folderName);
        }

        public Dictionary<string, List<string>> GetFormattingPrompts()
        {
            var folderName = "Formatting";

            ValidatePromptFolder(folderName);

            return GetPromptsFromFolder(folderName);
        }

        private Dictionary<string, List<string>> GetPromptsFromFolder(string folderName)
        {
            var prompts = Directory.GetFiles(Path.Combine(_promptsPath, folderName), "*.txt");
            Dictionary<string, List<string>> promptContents = new Dictionary<string, List<string>>();

            foreach (var promptFile in prompts)
            {
                var role = string.Empty;
                if (Path.GetFileName(promptFile).StartsWith("system_"))
                {
                    role = "system";
                }
                else if (Path.GetFileName(promptFile).StartsWith("user_"))
                {
                    role = "user";
                }
                else
                {
                    _logger.LogWarning("Prompt file {PromptFile} does not have a recognized role prefix. Defaulting to 'user'.", promptFile);
                    role = "user";
                }

                if (!promptContents.ContainsKey(role))
                {
                    promptContents[role] = new List<string>();
                }

                promptContents[role].Add(File.ReadAllText(promptFile));
            }
            return promptContents;
        }

        /// <summary>
        /// Validates that the specified prompt folder exists and contains at least one text file.
        /// </summary>
        /// <param name="folderName"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private void ValidatePromptFolder(string folderName)
        {
            var path = Path.Combine(_promptsPath, folderName);

            if (!Directory.Exists(path))
            {
                _logger.LogError("Prompt folder not found: {PromptPath}", path);

                throw new DirectoryNotFoundException($"Prompt folder not found: {path}");
            }

            if (Directory.GetFiles(path, "*.txt").Length == 0)
            {
                _logger.LogError("Prompt folder does not contain any text files: {PromptPath}", path);
                throw new InvalidOperationException($"Prompt folder does not contain any text files: {path}");
            }
        }
    }
}
