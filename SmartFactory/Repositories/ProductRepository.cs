using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SmartFactoryDbContext _context;

        public ProductRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken token)
        {
            return await _context.Products
                .OrderBy(x => x.ProductId)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(Product product)
        {
            var target = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductId == product.ProductId);

            if (target == null)
                return;

            target.ProductCode = product.ProductCode;
            target.ProductName = product.ProductName;
            target.Unit = product.Unit;

            await _context.SaveChangesAsync();
        }
    }
}
