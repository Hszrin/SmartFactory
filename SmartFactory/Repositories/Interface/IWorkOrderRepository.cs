using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface IWorkOrderRepository
    {
        Task<List<WorkOrder>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(WorkOrder workOrder);
    }
}
