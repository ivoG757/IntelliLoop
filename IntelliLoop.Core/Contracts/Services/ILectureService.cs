using IntelliLoop.Core.DTOs.Request;
using IntelliLoop.Core.DTOs.Response;
namespace IntelliLoop.Core.Contracts.Services
{
    public interface ILectureService
    {
        /// <summary>
        /// Saves the uploaded audio, queues a new processing job. Returns the processing job's id.
        /// </summary>
        /// <param name="request"></param>
        public Task<Guid> CreateJobAsync(UploadLectureRequestDto request);

        /// <summary>
        /// Returns the job's status and lecture title, 
        /// both can be null if either such job doesn't exist or lecture doesn't exist
        /// </summary>
        /// <param name="request"></param>
        public Task<JobResponseDto?> GetJobAsync(JobRequestDto request);

        /// <summary>
        /// Returns the file associated with the processing job
        /// </summary>
        /// <param name="jobRequestDto"></param>
        public Task<AudioResponseDto?> GetAudioAsync(JobRequestDto jobRequestDto);

        /// <summary>
        /// Returns a finished lecture by the lecture's job id. 
        /// Can be null if the lecture doesn't exist or isn't finished.
        /// </summary>
        /// <param name="request"></param>
        public Task<LectureResponseDto?> GetLectureAsync(JobRequestDto request);

        /// <summary>
        /// Returns a page of the user's jobs, sorted from newest to oldest.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ICollection<JobResponseDto>> GetJobsAsync(JobListRequestDto request);
    }
}
