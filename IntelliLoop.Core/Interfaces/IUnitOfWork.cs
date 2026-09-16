using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync();
    }
}
