using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Persistence.Repository;

public interface IGenericRepository<T>
{
    Task<List<T>> GetAll();
    Task<List<T>> GetAll(Expression<Func<T, bool>> predicate);
    Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> predicate);
    Task<bool> Add(T item);
    Task<bool> AddRange(T item);
    Task<bool> Update(T item);
    Task<bool> Delete(T item);
    Task<bool> SaveChanges();
}
