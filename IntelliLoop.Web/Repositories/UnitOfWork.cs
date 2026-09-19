using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntelliLoop.Web.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IntelliLoopDbContext _context;
        public UnitOfWork(IntelliLoopDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
