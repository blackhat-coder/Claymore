using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Persistence;

public class DataContextFactory : IDbContextFactory<DataContext>
{
    private readonly IDbContextFactory<DataContext> _pooledFactory;
    public DataContextFactory(IDbContextFactory<DataContext> pooledFactory)
    {
        _pooledFactory = pooledFactory;
    }

    public DataContext CreateDbContext()
    {
        return _pooledFactory.CreateDbContext();
    }

    public async Task<DataContext> CreateDbContextAsync()
    {
        return await _pooledFactory.CreateDbContextAsync();
    }
}
