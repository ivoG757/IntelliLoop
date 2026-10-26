# Transcription Service

The `TranscriptionService` converts uploaded audio recordings into text using Whisper. It prepares the audio in the correct format, processes it, and returns the resulting transcript.

## Configuration

When the service is created, it reads the following settings from the application configuration:

- `Whisper:ModelPath` — the path to the Whisper model file.
- `AppSettings:basePath` — the base folder used to locate the model file.
- `Whisper:Language` — the language Whisper should use. If no language is configured, it defaults to `auto`.

The service creates a Whisper factory and processor using these settings.

## TranscribeAudioAsync

The `LectureWorker` uses this method during lecture processing. It receives the path to the uploaded audio file and a cancellation token.

The method works in the following steps:

1. **Check the file:** It checks whether the audio file exists using `IFileStorage`. If the file is missing, it throws a `FileNotFoundException`.
2. **Convert the audio:** It uses FFmpeg to create a temporary WAV file with the audio settings required by Whisper, including the configured sample rate, mono audio, and PCM encoding.

| Constant                 | Value                                   | Purpose                                                                 |
| ------------------------ | --------------------------------------- | ----------------------------------------------------------------------- |
| `WavContentType`         | `audio/wav`                             | MIME type for WAV audio files.                                          |
| `WavExtension`           | `wav`                                   | File format used for converted audio.                                   |
| `WhisperSampleRate`      | `16000`                                 | Sets the audio sampling rate to 16 kHz for Whisper.                     |
| `MonoAudioArgument`      | `-ac 1`                                 | Converts audio to a single channel (mono).                              |
| `PcmAudioCodec`          | `pcm_s16le`                             | Uses 16-bit PCM audio encoding.                                         |
| `TempFileSuffix`         | `.temp`                                 | Identifies temporary converted audio files.                             |
| `MaxAudioFileSizeBytes`  | `100 MB`                                | Maximum allowed audio file size.                                        |
| `AllowedAudioExtensions` | `.mp3`, `.wav`, `.m4a`, `.ogg`, `.webm` | File extensions accepted by the application, regardless of letter case. |

3. **Read the converted file:** It opens the temporary file as a stream using `IFileStorage`.

4. **Transcribe the audio:** It passes the stream to the Whisper processor, which returns text in separate results.
5. **Combine the results:** Each text result is added to a `StringBuilder`. After processing finishes, the method returns the complete transcript as a string.
6. **Clean up:** A `finally` block deletes the temporary converted file if it still exists. This cleanup also runs if an error occurs during conversion or transcription.

The cancellation token is passed to Whisper's processing loop so the transcription can be cancelled when requested.

## Resource Cleanup

The service implements `IDisposable` to release the resources used by the Whisper processor and factory when the service is disposed.
