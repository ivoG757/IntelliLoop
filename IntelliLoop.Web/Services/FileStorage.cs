using IntelliLoop.Core.Interfaces;

namespace IntelliLoop.Web.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly string _basePath;

        public FileStorage(IConfiguration configuration)
        {
            _basePath = configuration["AppSettings:basePath"]!;
            Directory.CreateDirectory(_basePath);
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

        public async Task<string> SaveAsync(IFormFile file, string userId)
        {

            var userFolder = Path.Combine(
            _basePath, userId);

            Directory.CreateDirectory(userFolder);

            var extension = Path.GetExtension(file.FileName);
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
