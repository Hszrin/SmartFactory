using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly SmartFactoryDbContext _context;

        public WorkOrderRepository(SmartFactoryDbContext context)
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

            // 생산실적 합계를 한 번에 집계해 작업지시별 현재 생산량을 채운다.
            var productionTotals = await _context.ProductionResults
                .GroupBy(x => x.WorkOrderId)
                .Select(g => new
                {
                    WorkOrderId = g.Key,
                    TotalQuantity = g.Sum(x => x.ProductionQuantity)
                })
                .ToDictionaryAsync(
                    x => x.WorkOrderId,
                    x => x.TotalQuantity,
                    token);

            foreach (var order in orders)
            {
                order.CurrentQuantity = productionTotals.GetValueOrDefault(
                    order.WorkOrderId,
                    0);
            }

            return orders;
        }

        public async Task UpdateAsync(WorkOrder workOrder)
        {
            var target = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == workOrder.WorkOrderId);

            if (target == null)
                return;

            target.PlanId = workOrder.PlanId;
            target.ProductId = workOrder.ProductId;
            target.LineId = workOrder.LineId;
            target.TargetQuantity = workOrder.TargetQuantity;
            target.Status = workOrder.Status;
            target.StartTime = workOrder.StartTime;
            target.EndTime = workOrder.EndTime;

            await _context.SaveChangesAsync();
        }
    }
}
