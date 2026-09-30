using SmartFactory.Models;


namespace SmartFactory.Repositories.Interface
{
    public interface IProductionPlanRepository
    {
        Task<List<ProductionPlan>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(ProductionPlan plan);
    }
}
