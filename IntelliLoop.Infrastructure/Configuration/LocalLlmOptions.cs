using System.ComponentModel.DataAnnotations;

namespace IntelliLoop.Infrastructure.Configuration;

/// <summary>
/// Configuration for the local language-model pipeline.
/// Values are bound from the "LocalLlm" section in appsettings.json.
/// </summary>
public sealed class LocalLlmOptions
{
    public const string SectionName = "LocalLlm";

    /// <summary>
    /// Controls sampling randomness. Lower values are more consistent;
    /// Ollama's normal temperature range is 0 through 2.
    /// </summary>
    [Range(0.0, 2.0)]
    public float Temperature { get; set; } = 0.2f;

    /// <summary>
    /// Smallest context window (in tokens) this service will request, even for short prompts.
    /// A context window contains the prompt and generated tokens, not just the prompt.
    /// </summary>
    [Range(1024, 131072)]
    public int MinimumContextTokens { get; set; } = 8192;

    /// <summary>
    /// Hard upper bound (in tokens) on the context size requested by this service.
    /// Raising this may use substantially more RAM/VRAM. It is not a guarantee that
    /// the selected model and machine can run comfortably at that context size.
    /// </summary>
    [Range(1024, 131072)]
    public int MaximumContextTokens { get; set; } = 32768; //about 25 000 words

    /// <summary>
    /// Approximate number of characters that are treated as one token when estimating prompt size.
    /// the correct ratio varies by model, language, and text.
    /// </summary>
    [Range(1, 16)]
    public int CharactersPerTokenEstimate { get; set; } = 3; //rough estimate

    /// <summary>
    /// Context-window headroom planned for the output by the extraction request.
    /// This affects the chosen context size only; It does not place a restriction on the generated response.
    /// </summary>
    [Range(1, 131072)]
    public int ExtractionOutputReserveTokens { get; set; } = 8192;

    /// <summary>
    ///  Extra token headroom to absorb estimation errors.
    /// </summary>
    [Range(0, 8192)]
    public int ContextSafetyMarginTokens { get; set; } = 128;
    /// <summary>
    /// Context-window headroom planned for output by the formatting request.
    /// This will affect the chosen context size only; It does not place a restriction on the generated response.
    /// </summary>
    [Range(1, 131072)]
    public int FormattingOutputReserveTokens { get; set; } = 8192;

    /// <summary>
    /// Whether the Ollama specific request should enable the model's thinking mode.
    /// Defaults to false to avoid extra token usage. 
    /// Support and behavior depend on the selected model/provider.
    /// </summary>
    public bool EnableThinking { get; set; } = false;

    /// <summary>
    /// Whether to ask OllamaSharp to enforce the LectureAnalysis JSON Schema at generation.
    /// This is disabled by default because the current tested Ollama/model combination returned
    /// a "failed to parse grammar" error when constrained generation was enabled.
    /// </summary>
    public bool UseJsonSchemaResponseFormat { get; set; } = false;

    /// <summary>
    /// Context sizes are rounded up to a multiple of this number. This gives the sizing
    /// algorithm predictable increments; it is a design choice, its not a requirement imposed by Ollama.
    /// </summary>
    [Range(1, 8192)]
    public int ContextRoundingMultipleTokens { get; set; } = 1024;
}
