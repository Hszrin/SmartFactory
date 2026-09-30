using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface ILineRepository
    {
        Task<List<ProductionLine>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(ProductionLine line);
    }
}