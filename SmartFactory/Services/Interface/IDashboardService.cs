using SmartFactory.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
