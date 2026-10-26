# IntelliLoop's AI Processing

IntelliLoop processes uploaded lecture recordings in several stages. The application separates file storage, background processing, transcription, AI analysis, and database operations into different components.

## Architecture Overview

The application uses a background processing architecture so that audio transcription and AI analysis can run separately from the upload request.

The main components are:

- **File storage:** Stores the uploaded audio and provides access to the files.
- **Repository** Stores processing jobs through the database and, after successful processing, the generated lecture and its content.
- **Channel:** Acts as a queue for job IDs waiting to be processed.
- **LectureWorker:** Reads job IDs from the Channel and starts the processing workflow.
- **LectureProcessingService:** Coordinates transcription, AI analysis, and saving the results.
- **TranscriptionService:** Converts audio into text using Whisper.
- **LocalLlmService:** Turns the transcript into structured lecture content using a local AI model.

The services have separate responsibilities, which makes the processing workflow easier to maintain and allows individual parts to be changed without rewriting the entire pipeline.

## Workflow

### 1. Upload and Queue

When a user uploads an audio file, the application stores the file and creates a processing job in the database. The job ID is then placed in the processing `Channel`.

The `LectureWorker` reads the job ID from the Channel and passes it to the processing service. This allows the time-consuming work to run in the background instead of keeping the upload request running until the lecture is ready.

### 2. Transcription

The processing service uses [`TranscriptionService`](transcription-service.md) to convert the uploaded audio into text using Whisper.

The service first converts the audio into a format suitable for Whisper. It then processes the audio and combines the recognized text into a single transcript.

### 3. Lecture Analysis

The processing service sends the transcript to [`LocalLlmService`](local-llm-pipeline.md), which uses a local AI model to generate structured lecture content.

This stage makes two AI requests. The first extracts important information from the transcript. The second organizes that information into a `LectureAnalysis` object. The generated result is validated before being returned to the processing workflow.

### 4. Saving the Results

After processing finishes successfully, the lecture analysis is saved to the database, and the processing job's status is updated.

The user can then open the generated lecture through the application.

Currently, the application does not automatically update the user interface through SignalR or polling. The user may need to refresh the page to see the latest processing status or results.
