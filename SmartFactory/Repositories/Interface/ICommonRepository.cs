using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Repositories.Interface
{
    public interface ICommonRepository<T, TKey>
    where T : class
    {
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task DeleteAsync(TKey id);
    }
}
