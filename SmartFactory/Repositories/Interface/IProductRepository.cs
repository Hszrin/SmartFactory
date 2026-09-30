using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(Product product);
    }
}