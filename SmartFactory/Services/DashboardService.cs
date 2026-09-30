using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Dtos;
using SmartFactory.Repositories.Interface;
using SmartFactory.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly SmartFactoryDbContext _context;
        public DashboardService(
            SmartFactoryDbContext context)
        {
            _context = context;
        }
        public async Task<DashboardSummaryDto> GetSummaryAsync(DateTime start, DateTime end, CancellationToken token)
        {
            var dto = new DashboardSummaryDto();

            var todayResults = _context.ProductionResults
                .Where(x =>
                    x.ProductionTime >= start &&
                    x.ProductionTime < end);

            var totalProduction = await todayResults
                .SumAsync(x => x.ProductionQuantity);

            var goodQuantity = await todayResults
                .SumAsync(x => x.GoodQuantity);

            var defectQuantity = await todayResults
                .SumAsync(x => x.DefectQuantity);

            var runningWorkOrderCount = await _context.WorkOrders
                .CountAsync(x => x.Status == "RUNNING");

            var completedWorkOrderCount = await _context.WorkOrders
                .CountAsync(x => x.Status == "COMPLETED");


            return new DashboardSummaryDto
            {
                TotalProduction = totalProduction,
                GoodQuantity = goodQuantity,
                DefectQuantity = defectQuantity,
                DefectRate = totalProduction == 0
                    ? 0
                    : (float)defectQuantity / totalProduction * 100,
                RunningWorkOrderCount = runningWorkOrderCount,
                CompletedWorkOrderCount = completedWorkOrderCount
            };
        }
        public async Task<List<MachineProductionDto>> GetMachineProductionAsync(DateTime start, DateTime end, CancellationToken token)
        {
            return await _context.ProductionResults
                .Where(x => x.ProductionTime >= start &&
                            x.ProductionTime < end)
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
        public async Task<List<ProductProductionDto>> GetProductProductionAsync(DateTime start, DateTime end, CancellationToken token)
        {
            return await _context.ProductionResults
                .Where(x => x.ProductionTime >= start &&
                            x.ProductionTime < end)
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
        public async Task<List<DefectTypeDto>> GetDefectProductionAsync(DateTime start, DateTime end, CancellationToken token)
        {
            return await _context.Defects
                .Where(x => x.Result!.ProductionTime >= start &&
                            x.Result!.ProductionTime < end)
                .GroupBy(x => x.DefectType)
                .Select(g => new DefectTypeDto
                {
                    DefectType = g.Key,
                    Quantity = g.Sum(x => x.DefectQuantity)
                })
                .ToListAsync(token);
        }
        public async Task<List<WorkOrderProgressDto>> GetWorkOrderProgressAsync(CancellationToken t)
        {
            return await _context.WorkOrders
                .Select(w => new WorkOrderProgressDto
                {
                    WorkOrderId = w.WorkOrderId,
                    ProductName = w.Product!.ProductName,
                    TargetQuantity = w.TargetQuantity,
                    Status = w.Status,

                    CurrentQuantity = _context.ProductionResults
                        .Where(r => r.WorkOrderId == w.WorkOrderId)
                        .Sum(r => (int?)r.ProductionQuantity) ?? 0,

                    ProgressRate =
                        w.TargetQuantity == 0
                            ? 0
                            : (double)(
                                _context.ProductionResults
                                    .Where(r => r.WorkOrderId == w.WorkOrderId)
                                    .Sum(r => (int?)r.ProductionQuantity) ?? 0
                            )
                            / w.TargetQuantity * 100
                })
                .ToListAsync(t);
        }
    }
}
