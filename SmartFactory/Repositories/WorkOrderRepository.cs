using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Numerics;

namespace SmartFactory.Repositories
{
    internal class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly SmartFactoryDbContext _context;
        public WorkOrderRepository(
            SmartFactoryDbContext context)
        {
            _context = context;
        }
        public async Task<List<WorkOrder>> GetAllAsync(CancellationToken token)
        {
            var orders = await _context.WorkOrders
                .Include(x => x.Plan)
                .Include(x => x.Product)
                .Include(x => x.Line)
                .OrderBy(x => x.WorkOrderId)
                .ToListAsync(token);

            var productionTotals =
                await _context.ProductionResults
                    .GroupBy(x => x.WorkOrderId)
                    .Select(g => new
                    {
                        WorkOrderId = g.Key,
                        TotalQuantity =
                            g.Sum(x => x.ProductionQuantity)
                    })
                    .ToDictionaryAsync(
                        x => x.WorkOrderId,
                        x => x.TotalQuantity);

            foreach (var order in orders)
            {
                order.CurrentQuantity =
                    productionTotals.GetValueOrDefault(
                        order.WorkOrderId,
                        0);
            }

            return orders;
        }
        public async Task UpdateAsync(WorkOrder workOrder)
        {
            var target = await _context.WorkOrders
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrder.WorkOrderId);

            // 해당 ProductId가 DB에 존재하지 않는 경우
            if (target == null)
                return;

            target.ProductId = workOrder.ProductId;
            target.PlanId = workOrder.PlanId;
            target.LineId = workOrder.LineId;
            target.TargetQuantity = workOrder.TargetQuantity;
            target.StartTime = workOrder.StartTime;
            target.EndTime = workOrder.EndTime;
            target.CreatedAt = workOrder.CreatedAt;
            target.Status = workOrder.Status;

            await _context.SaveChangesAsync();
        }
    }
}
