using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface IFileStorage
    {
        public Task Delete(string path);
        public bool Exists(string path);
        public Stream GetStream(string path);
        public Task<string> SaveAsync(IFormFile file, string userId);
    }
}
