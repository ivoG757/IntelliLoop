using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Contracts.Services
{
    public interface IWhisperModelService
    {
        public Task EnsureModelExistsAsync(CancellationToken cancellationToken = default);
    }
}
