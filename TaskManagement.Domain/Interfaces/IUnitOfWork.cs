public interface IUnitOfWork : IDisposable
{
    ITaskRepository Tasks { get; }
    IRepository<Project> Projects { get; }
    IRepository<AppUser> Users { get; }
    
    Task<int> SaveChangesAsync();
}