using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface IProductionResultRepository
    {
        Task<List<ProductionResult>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(ProductionResult result);
    }
}