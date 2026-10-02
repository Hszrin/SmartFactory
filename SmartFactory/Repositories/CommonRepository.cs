using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class CommonRepository<T, TKey> : ICommonRepository<T, TKey>
        where T : class
    {
        private readonly SmartFactoryDbContext _context;

        public CommonRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
