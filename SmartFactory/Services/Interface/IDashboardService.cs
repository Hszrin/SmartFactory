using SmartFactory.Dtos;

namespace SmartFactory.Services.Interface
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(DateTime start, DateTime end, CancellationToken token);
        Task<List<MachineProductionDto>> GetMachineProductionAsync(DateTime start, DateTime end, CancellationToken token);
        Task<List<ProductProductionDto>> GetProductProductionAsync(DateTime start, DateTime end, CancellationToken token);
        Task<List<DefectTypeDto>> GetDefectProductionAsync(DateTime start, DateTime end, CancellationToken token);
        Task<List<WorkOrderProgressDto>> GetWorkOrderProgressAsync(CancellationToken token);
    }
}
