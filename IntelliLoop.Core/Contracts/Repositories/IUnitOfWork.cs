namespace IntelliLoop.Core.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync();
    }
}
