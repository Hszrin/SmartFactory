using SmartFactory.Models;

namespace SmartFactory.Services.Interface
{
    public interface IProductionService
    {
        Task RegisterResultAsync(ProductionResult result);
    }
}