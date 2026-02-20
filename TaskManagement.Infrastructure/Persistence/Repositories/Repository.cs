using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

// This single class provides CRUD for ANY entity in our system.
// Without this, you'd write the same GetById, Add, Update, Delete
// code separately for Tasks, Projects, Users... forever.
public class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        // FindAsync is optimized — it checks the EF Core change tracker
        // first (in-memory cache) before hitting the database.
        // If you loaded this entity earlier in the same request,
        // it won't query the database again.
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        // AsNoTracking() is critical for read-only queries.
        // By default EF Core tracks every entity it loads,
        // storing a snapshot to detect changes later.
        // For queries where you just need to READ data,
        // tracking wastes memory and CPU. AsNoTracking skips it.
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        // Notice: we do NOT call SaveChangesAsync here.
        // That's the Unit of Work's job.
        // This allows multiple operations to be batched
        // into a single database transaction.
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        // Attach tells EF Core "this entity exists in the database"
        // Then we mark it as Modified so EF generates an UPDATE statement
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        // AnyAsync is more efficient than GetById for existence checks.
        // It generates SELECT TOP 1 1 FROM table WHERE Id = @id
        // instead of selecting all columns.
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}
