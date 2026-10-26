# IntelliLoop

> Turn lecture recordings into structured study materials.

IntelliLoop is an ASP.NET Core MVC web application that transforms lecture audio recordings into study-material content. It combines speech recognition with a locally hosted large language model (LLM) to transcribe lectures and generate summaries, key concepts, and study notes.

The project aims to support a broader learning loop, from lecture processing and study-material generation to practice quizzes and grading. The current version focuses on the lecture-to-study-material workflow; quiz generation and grading are not yet implemented.

## Features

- **User authentication** — Registration and authentication using ASP.NET Core Identity.
- **Lecture audio upload** — Upload audio recordings for asynchronous processing.
- **Background processing** — Queue lecture-processing jobs and monitor their progress without keeping the upload request open.
- **Speech-to-text transcription** — Convert recorded speech into text using Whisper.
- **AI-generated study materials** — Use a locally hosted LLM to extract relevant information and generate structured summaries, key concepts, and study notes.
- **Processing status tracking** — Track processing jobs and their progress, including completed and failed jobs.
- **Lecture management** — View, edit, and delete generated lectures.
- **Persistent storage** — Store application data using Entity Framework Core and Microsoft SQL Server.

## How It Works

IntelliLoop separates lecture submission from the processing workflow so that transcription and AI analysis can run in the background.

### Processing Pipeline

1. **Submission:** The uploaded recording is saved, and a processing job is created in the database.
2. **Background execution:** The job is queued for processing, and the user is redirected to a page where progress can be monitored.
3. **Audio processing:** FFmpeg is used for audio preprocessing and format conversion where required.
4. **Transcription:** Whisper converts the recording into a text transcript.
5. **Information extraction:** The LLM extracts relevant information from the transcript.
6. **Study-material generation:** A second LLM request organizes the extracted information into the application's structured lecture format.
7. **Persistence:** The generated lecture and its associated data are saved and become available through the application.

The two-stage LLM pipeline separates information extraction from output formatting. This makes the two responsibilities easier to inspect and debug, although it does not eliminate incorrect or hallucinated output.

## Technology Stack

| Technology | Purpose |
| --- | --- |
| C# and .NET 10 | Application runtime and development platform |
| ASP.NET Core MVC | Web application architecture and request handling |
| Razor Views | Server-rendered user interface |
| Bootstrap | Responsive styling and UI components |
| Entity Framework Core | Object-relational mapping and database access |
| Microsoft SQL Server | Persistent application data |
| ASP.NET Core Identity | User authentication and account management |
| Whisper.net | Speech-to-text transcription |
| FFmpeg and FFMpegCore | Audio preprocessing and conversion |
| Ollama | Local large language model runtime |
| OllamaSharp and Microsoft.Extensions.AI | Integration with the language model |
| Dependency Injection | Service registration and dependency management |

## Getting Started

### Prerequisites

Before running IntelliLoop, prepare the following:

- **.NET 10 SDK** — Required to build and run the application.
- **Microsoft SQL Server or SQL Server LocalDB** — Used to persist application data.
- **Ollama** — Runs the language model used for lecture analysis.
- **FFmpeg** — Required for supported audio-processing operations.
- **Internet connection for initial setup** — The configured Whisper model is downloaded if it is not already available locally.

The processing time and resource requirements depend on the selected transcription model, LLM, recording length, and available hardware.

### 1. Clone the Repository

```bash
git clone https://github.com/ivoG757/IntelliLoop.git
cd IntelliLoop
```

### 2. Prepare the Language Model

Install Ollama from [ollama.com](https://ollama.com), then download the configured model:

```bash
ollama pull qwen3:8b
```

Ensure Ollama is running and that the model is available locally. The default endpoint used by the application is:

```text
http://localhost:11434
```

If Ollama runs on another machine, configure the application to use that machine's reachable endpoint instead.

### 3. Install FFmpeg

Install FFmpeg using the instructions for your operating system at [ffmpeg.org](https://ffmpeg.org/download.html).

Ensure that the FFmpeg executable is accessible to the application. If FFmpeg is not available through the system `PATH`, configure its location according to your local setup.

### 4. Configure the Application

The web project reads its configuration from `IntelliLoop.Web/appsettings.json` and the standard ASP.NET Core configuration providers.

For local development, use `IntelliLoop.Web/appsettings.Development.json` for machine-specific settings. The following example assumes Windows, SQL Server LocalDB, and a Whisper model configuration that supports selecting the model by name.

- Sample:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=IntelliLoopDb;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },

  "Identity": {
    "Password": {
      "RequireNonAlphanumeric": false,
      "RequiredUniqueChars": 1,
      "RequireUppercase": false,
      "RequireLowercase": false,
      "RequireDigit": false
    },
    "Lockout": {
      "MaxFailedAccessAttempts": 10,
      "DefaultLockoutTimeSpan": "00:15:00",
      "AllowedForNewUsers": true
    }
  },

  "AllowedHosts": "*",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "qwen3:8b",
    "Timeout": 15
  },
  "Whisper": {
    "ModelPath": "Models/ggml-Small.bin",
    "Model": "Small",
    "Language": "auto"
  },
  "Prompts": {
    "Path": "Prompts"
  },
  "LocalLlm": {
    "Temperature": 0.2,
    "MinimumContextTokens": 8192,
    "MaximumContextTokens": 32768,
    "CharactersPerTokenEstimate": 3,
    "ExtractionOutputReserveTokens": 8192,
    "FormattingOutputReserveTokens": 8192,
    "ContextSafetyMarginTokens": 128,
    "ContextRoundingMultipleTokens": 1024,
    "EnableThinking": false,
    "UseJsonSchemaResponseFormat": false
  },
  "AppSettings": {
    "basePath": "D:\\IntelliLoop"
  },
  "FileStorage": {
    "Path": "UserFiles"
  }
}
```

Adjust the connection string to match your SQL Server installation. The configured base directory must be writable by the application.

The `Whisper:ModelPath` is resolved relative to `AppSettings:basePath` in the model service. Keep the selected model and its file path consistent.

The application requires the `Identity` settings to be present in the configuration.

**Configuration note:** The application reads `ConnectionStrings:DefaultConnection` for its database context. Use that key for the connection string rather than relying on a second, separately named connection string.

### 5. Apply Database Migrations

Apply Entity Framework Core migrations into your configured database(The migration command assumes the migrations are stored in `IntelliLoop.Infrastructure`.):

```bash
dotnet ef database update --project IntelliLoop.Infrastructure --startup-project IntelliLoop.Web
```

If the `dotnet ef` command is unavailable, install the EF Core command-line tool:

```bash
dotnet tool install --global dotnet-ef
```

### 6. Build and Run

From the repository root, restore dependencies and build the solution:

```bash
dotnet restore IntelliLoop.slnx
dotnet build IntelliLoop.slnx
```

Start the web application:

```bash
dotnet run --project IntelliLoop.Web
```

On startup, IntelliLoop checks whether the configured Whisper model exists. If it is missing, the application downloads it. The first startup can therefore take much longer than standart launches.

Once the application starts Visual studio should open the app URL on a new tab in your browser, if not, open the local URL printed in the console.

## Configuration Reference

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection used by Entity Framework Core |
| `Ollama:BaseUrl` | Base URL of the Ollama service |
| `Ollama:Model` | Model selected for LLM analysis |
| `Whisper:Model` | Selected Whisper model, where supported by the configured implementation |
| `Whisper:ModelPath` | Path to the downloaded Whisper model |
| `Whisper:Language` | Transcription language setting; `auto` enables automatic language detection where supported |
| `AppSettings:basePath` | Base directory used by the application for model-related paths |
| `Prompts:Path` | Location of the LLM prompt files |
| `FileStorage:Path` | Configured application file-storage location |

Keep machine-specific paths and credentials out of committed configuration files. Use development-specific settings or .NET user secrets for local configuration where appropriate.

## Project Structure

The solution is split into projects with distinct responsibilities.

```text
IntelliLoop/
├── IntelliLoop.Common/
├── IntelliLoop.Core/
├── IntelliLoop.Infrastructure/
├── IntelliLoop.Tests/
├── IntelliLoop.Web/
└── IntelliLoop.slnx
```

- **IntelliLoop.Common** — Shared constants and exception-related definitions.
- **IntelliLoop.Core** — Core entities, DTOs, service contracts, and application abstractions.
- **IntelliLoop.Infrastructure** — Data access, persistence, and integrations with transcription and language-model services.
- **IntelliLoop.Tests** — Automated tests.
- **IntelliLoop.Web** — ASP.NET Core MVC application, Razor views, authentication, configuration, and application startup.

##Current Limitations

- **Generated content requires review.** Transcription errors and LLM-generated summaries may omit information, misinterpret context, or contain inaccuracies.
- **Results depend on input quality.** Recording quality, speech clarity, transcript length, and model selection can affect processing time and output quality.
- **Background processing is not yet fully resilient to restarts.** The current job queue uses an in-process channel, so queued or interrupted work is not guaranteed to resume automatically after an application restart.
- **Quiz generation and grading are not yet implemented.** They belong to the broader learning workflow envisioned for IntelliLoop but are not included the current version. 
- Local processing can be resource-intensive and times vary significantly depending on running machine and app configuration.
- Tests are not yet implemented.


## Documentation
- [Architecture](docs/architecture.md)
- [Local LLM Pipeline](docs/local-llm-pipeline.md)
- [Transcription Service](docs/transcription-service.md)
