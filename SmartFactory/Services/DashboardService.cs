using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Dtos;
using SmartFactory.Services.Interface;

namespace SmartFactory.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly SmartFactoryDbContext _context;

        public DashboardService(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(
            DateTime start,
            DateTime end,
            CancellationToken token)
        {
            var periodResults = _context.ProductionResults
                .Where(x => x.ProductionTime >= start && x.ProductionTime < end);

            var totalProduction = await periodResults
                .SumAsync(x => x.ProductionQuantity, token);
            var goodQuantity = await periodResults
                .SumAsync(x => x.GoodQuantity, token);
            var defectQuantity = await periodResults
                .SumAsync(x => x.DefectQuantity, token);

            var runningWorkOrderCount = await _context.WorkOrders
                .CountAsync(x => x.Status == "RUNNING", token);
            var completedWorkOrderCount = await _context.WorkOrders
                .CountAsync(x => x.Status == "COMPLETED", token);

            return new DashboardSummaryDto
            {
                TotalProduction = totalProduction,
                GoodQuantity = goodQuantity,
                DefectQuantity = defectQuantity,
                DefectRate = totalProduction == 0
                    ? 0
                    : (double)defectQuantity / totalProduction * 100,
                RunningWorkOrderCount = runningWorkOrderCount,
                CompletedWorkOrderCount = completedWorkOrderCount
            };
        }

        public async Task<List<MachineProductionDto>> GetMachineProductionAsync(
            DateTime start,
            DateTime end,
            CancellationToken token)
        {
            return await _context.ProductionResults
                .Where(x => x.ProductionTime >= start && x.ProductionTime < end)
                .GroupBy(x => new
                {
                    x.MachineId,
                    x.Machine!.MachineName
                })
                .Select(g => new MachineProductionDto
                {
                    MachineName = g.Key.MachineName,
                    ProductionQuantity = g.Sum(x => x.ProductionQuantity)
                })
                .ToListAsync(token);
        }

        public async Task<List<ProductProductionDto>> GetProductProductionAsync(
            DateTime start,
            DateTime end,
            CancellationToken token)
        {
            return await _context.ProductionResults
                .Where(x => x.ProductionTime >= start && x.ProductionTime < end)
                .GroupBy(x => new
                {
                    x.WorkOrder!.ProductId,
                    x.WorkOrder!.Product!.ProductName
                })
                .Select(g => new ProductProductionDto
                {
                    ProductName = g.Key.ProductName,
                    ProductionQuantity = g.Sum(x => x.ProductionQuantity)
                })
                .ToListAsync(token);
        }

        public async Task<List<DefectTypeDto>> GetDefectProductionAsync(
            DateTime start,
            DateTime end,
            CancellationToken token)
        {
            return await _context.Defects
                .Where(x =>
                    x.Result!.ProductionTime >= start &&
                    x.Result.ProductionTime < end)
                .GroupBy(x => x.DefectType)
                .Select(g => new DefectTypeDto
                {
                    DefectType = g.Key,
                    Quantity = g.Sum(x => x.DefectQuantity)
                })
                .ToListAsync(token);
        }

        public async Task<List<WorkOrderProgressDto>> GetWorkOrderProgressAsync(
            CancellationToken token)
        {
            var productionTotals = await _context.ProductionResults
                .GroupBy(x => x.WorkOrderId)
                .Select(g => new
                {
                    WorkOrderId = g.Key,
                    CurrentQuantity = g.Sum(x => x.ProductionQuantity)
                })
                .ToDictionaryAsync(
                    x => x.WorkOrderId,
                    x => x.CurrentQuantity,
                    token);

            var workOrders = await _context.WorkOrders
                .Select(w => new
                {
                    w.WorkOrderId,
                    ProductName = w.Product!.ProductName,
                    w.TargetQuantity,
                    w.Status
                })
                .OrderBy(x => x.WorkOrderId)
                .ToListAsync(token);

            return workOrders
                .Select(w =>
                {
                    var currentQuantity = productionTotals.GetValueOrDefault(w.WorkOrderId, 0);

                    return new WorkOrderProgressDto
                    {
                        WorkOrderId = w.WorkOrderId,
                        ProductName = w.ProductName,
                        TargetQuantity = w.TargetQuantity,
                        CurrentQuantity = currentQuantity,
                        Status = w.Status,
                        ProgressRate = w.TargetQuantity == 0
                            ? 0
                            : (double)currentQuantity / w.TargetQuantity * 100
                    };
                })
                .ToList();
        }
    }
}
