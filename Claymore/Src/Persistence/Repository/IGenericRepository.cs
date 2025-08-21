using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Persistence.Repository;

public interface IGenericRepository<T>
{
    Task<List<T>> GetAll(DataContext? context = null);
    Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, DataContext? context = null);
    Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> predicate, DataContext? context = null);
    Task<bool> Add(T item, DataContext? context = null);
    Task<bool> AddRange(T item, DataContext? context = null);
    Task<bool> Update(T item, DataContext? context = null);
    Task<bool> Delete(T item, DataContext? context = null);
    Task<bool> SaveChanges(DataContext? context = null);
}
