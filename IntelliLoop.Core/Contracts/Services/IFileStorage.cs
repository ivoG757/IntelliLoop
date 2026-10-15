namespace IntelliLoop.Core.Contracts.Services
{
    public interface IFileStorage
    {
        public Task Delete(string path);
        public bool Exists(string path);
        public Stream GetStream(string path);
        public Task<string> SaveAsync(Stream content, string fileName, string userId);
    }
}
