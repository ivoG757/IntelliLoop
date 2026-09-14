using IntelliLoop.Core.Interfaces;
using System.Text;

namespace IntelliLoop.Web.Services
{
    public class PromptService : IPromptService
    {
        private readonly ILogger<PromptService> _logger;
        private readonly string _promptPath;

        public PromptService(IConfiguration configuration, ILogger<PromptService> logger, IHostEnvironment environment)
        {
            _logger = logger;

            var promptPath = configuration["Prompts:Path"] ?? throw new ArgumentNullException("Prompts:Path configuration is missing.");

            _promptPath = Path.Combine(environment.ContentRootPath, promptPath);
        }

        public string GetPrompts()
        {

            if (!Directory.Exists(_promptPath))
            {
                _logger.LogError("Prompt folder not found: {PromptPath}", _promptPath);

                throw new DirectoryNotFoundException($"Prompt folder not found: {_promptPath}");
            }
            var prompts = Directory.GetFiles(_promptPath, "*.txt");

            if (prompts.Length == 0) 
            {
                _logger.LogError("Prompt folder does not contain any text files: {PromptPath}", _promptPath);
                throw new DirectoryNotFoundException($"Prompt folder does not contain any text files: {_promptPath}");
            }

            var stringBuilder = new StringBuilder();

            foreach (var promptFile in prompts)
            {
                stringBuilder.AppendLine(File.ReadAllText(promptFile));
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
