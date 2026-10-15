using IntelliLoop.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using IntelliLoop.Common.ExceptionMessages;

namespace IntelliLoop.Infrastructure.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly string _path;

        public FileStorage(IConfiguration configuration)
        {
            var basePath = configuration["AppSettings:basePath"] ?? throw new InvalidOperationException(Configuration.BasePathNotConfigured);
            var fileStoragePath = configuration["FileStorage:Path"] ?? throw new InvalidOperationException(Configuration.FileStoragePathNotConfigured);
            _path = Path.Combine(basePath, fileStoragePath);
        }

        public async Task Delete(string path)
        {
            await Task.Run(() => File.Delete(path));
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public Stream GetStream(string path)
        {
            var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            return stream;
        }

        public async Task<string> SaveAsync(Stream file, string fileName, string userId)
        {

            var userFolder = Path.Combine(
            _path, userId);

            Directory.CreateDirectory(userFolder);

            var extension = Path.GetExtension(fileName);
            var storedName = $"{Guid.NewGuid():N}{extension}";

            var path = Path.Combine(userFolder, storedName);

            await using var fileStream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None
            );

            await file.CopyToAsync(fileStream);

            return path;
        }
    }
}
