# Local LLM Pipeline

`LocalLlmService` turns a lecture transcript into a `LectureAnalysis` object. It sends two requests to the local AI model through `IChatClient`.

1. **Extraction:** The service combines the extraction prompt with the transcript and asks the model to find the important information from the lecture.

2. **Formatting:** The service sends the extracted information to the model again and asks it to return the data in the `LectureAnalysis` format.

3. **Validation:** Before returning the result, the service checks whether the generated object follows its validation rules. It also checks each `NoteAnalysis` object separately.

## Workflow

Both extraction and formatting get their prompts from `IPromptService`. They then pass their messages to the `BuildOptions` method, which creates the settings for each AI request.

`BuildOptions` uses the values from `LocalLlmOptions` to estimate how much context the request needs. It includes the prompt and a reserve for the generated response, then rounds the result up to the configured increment. If the required context exceeds the configured maximum, the method throws an exception.

### Configuration

The `LocalLlmOptions` class stores the settings used by `LocalLlmService`. These values can be changed in the application configuration without changing the service's code.

| Setting                         | Default | Meaning                                                                                                 |
| ------------------------------- | ------: | ------------------------------------------------------------------------------------------------------- |
| `Temperature`                   |   `0.2` | Controls how varied the model's responses are. Lower values generally produce more predictable results. |
| `MinimumContextTokens`          |  `8192` | The smallest context window the service will request.                                                   |
| `MaximumContextTokens`          | `32768` | The largest context window the service is allowed to request.                                           |
| `CharactersPerTokenEstimate`    |     `3` | Estimates the number of tokens based on the number of characters. This is only an approximation.        |
| `ExtractionOutputReserveTokens` |  `8192` | The amount of context space reserved for the extraction response.                                       |
| `FormattingOutputReserveTokens` |  `8192` | The amount of context space reserved for the formatting response.                                       |
| `ContextSafetyMarginTokens`     |   `128` | A margin used to detect when the context window is nearly full.                                         |
| `ContextRoundingMultipleTokens` |  `1024` | The increment used when rounding up the estimated context size.                                         |
| `EnableThinking`                | `false` | Controls whether the model's thinking mode is enabled through Ollama.                                   |
| `UseJsonSchemaResponseFormat`   | `false` | Controls whether the formatting request uses JSON Schema to constrain the model's response.             |

The default values are used when no configuration overrides are provided.

### Checking the response

After each request, `CheckResponse` logs the estimated context size, token usage, and reason the model stopped generating. It throws an exception if the response appears to have been cut off or the context window is nearly full.

After formatting, `IsAnalysisInConstraints` checks that the concepts and notes collections are present, that concepts are not empty, and that each note and the main analysis object pass their Data Annotation validation rules.

## Limitations

The context size is based on an estimate, not an exact token count. Different text can use different numbers of tokens, so the estimate may not always be accurate.

If the estimated context exceeds `MaximumContextTokens`, the service throws an exception instead of sending a request that may not fit. This helps prevent problems caused by oversized requests, but it does not guarantee that the model will never truncate text or produce incorrect information.

JSON Schema enforcement is disabled by default because it caused an error with the current Ollama setup. Without it, the model is not forced to follow the expected structure. The application still validates the result, but validation cannot guarantee that the generated information is factually correct.

Currently, when processing fails, the worker catches the exception, but the user is not shown the specific reason for the failure. In the future, providing a clear error message in the application would make these failures easier to understand and troubleshoot.
