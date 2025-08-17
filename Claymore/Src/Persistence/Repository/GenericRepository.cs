using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Persistence.Repository;

public class GenericRepository<T>
    : IGenericRepository<T>, IDisposable where T : class
{
    private DataContext _dbContext;
    private DbSet<T> _dbSet;
    private bool _isDisposed;

    public GenericRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
        _isDisposed = false;
    }

    public void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }
    public Task<bool> Add(T item)
    {
        _dbSet.Add(item);
        return Task.FromResult(true);
    }

    public Task<bool> Update(T item)
    {
        _dbSet.Update(item);
        return Task.FromResult(true);
    }

    public Task<bool> AddRange(T item)
    {
        _dbSet.AddRange(item);
        return Task.FromResult(true);
    }

    public Task<bool> Delete(T item)
    {
        _dbSet.Remove(item);
        return Task.FromResult(true);
    }

    public async Task<List<T>> GetAll()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
