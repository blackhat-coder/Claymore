using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private DataContextFactory _dbContextFactory;
    private bool _isDisposed;
    private ILogger<GenericRepository<T>> _logger;
    private DataContext? _dbContext;

    public GenericRepository(DataContextFactory dbContextFactory, ILogger<GenericRepository<T>> logger)
    {
        _dbContextFactory = dbContextFactory;
        _isDisposed = false;
        _logger = logger;
    }

    public void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task<DataContext> GetContext(DataContext? context)
    {
        if (context != null) return context;
            
        if(_dbContext == null)
            _dbContext = await _dbContextFactory.CreateDbContextAsync();

        return _dbContext;
    }

    public async Task<bool> Add(T item, DataContext? context = null)
    {
        var dbContext = await GetContext(context);
        
        dbContext.Set<T>().Add(item);
        return true;
    }

    public async Task<bool> Update(T item, DataContext? context = null)
    {
        var dbContext = await GetContext(context);

        dbContext.Set<T>().Update(item);
        return true;
    }

    public async Task<bool> AddRange(T item, DataContext? context = null)
    {
        var dbContext = await GetContext(context);

        dbContext.Set<T>().AddRange(item);
        return true;
    }

    public async Task<bool> Delete(T item, DataContext? context = null)
    {
        var dbContext = await GetContext(context);

        dbContext.Set<T>().Remove(item);
        return true;
    }

    public async Task<List<T>> GetAll(DataContext? context = null)
    {
        var dbContext = await GetContext(context);
        return await dbContext.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, DataContext? context = null)
    {
        var dbContext = await GetContext(context);
        return await dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> predicate, DataContext? context = null)
    {
        var dbContext = await GetContext(context);
        return await dbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> SaveChanges(DataContext? context = null)
    {
        try
        {
            var dbContext = await GetContext(context);
            await dbContext.SaveChangesAsync();
            return true;
        }catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
}
