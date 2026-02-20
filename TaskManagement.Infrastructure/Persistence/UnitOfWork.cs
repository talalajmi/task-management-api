using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence.Repositories;

namespace TaskManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    // Lazy initialization — repositories are only created when first accessed.
    // If a request only touches Tasks, the Projects repository
    // is never instantiated. Saves memory.
    private ITaskRepository? _tasks;
    private IRepository<Project>? _projects;
    private IRepository<AppUser>? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);

    public IRepository<Project> Projects => _projects ??= new Repository<Project>(_context);

    public IRepository<AppUser> Users => _users ??= new Repository<AppUser>(_context);

    public async Task<int> SaveChangesAsync()
    {
        // This is the single point where ALL pending changes
        // get committed to the database in one transaction.
        // If any operation fails, none of them are saved.
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        // Properly dispose the DbContext when the Unit of Work
        // goes out of scope, releasing the database connection.
        _context.Dispose();
    }
}
