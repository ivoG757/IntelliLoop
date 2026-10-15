using IntelliLoop.Core.Contracts.Repositories;
namespace IntelliLoop.Infrastructure.Data.Repositories
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
