using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Services.Interface;

namespace SmartFactory.Services
{
    public class ProductionService : IProductionService
    {
        private readonly SmartFactoryDbContext _context;

        public ProductionService(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task RegisterResultAsync(ProductionResult result)
        {
            // 생산실적 등록과 작업지시 완료 처리를 하나의 트랜잭션으로 묶어 정합성을 보장한다.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var workOrder = await _context.WorkOrders
                    .FirstOrDefaultAsync(x => x.WorkOrderId == result.WorkOrderId);

                if (workOrder == null)
                    throw new Exception("작업지시를 찾을 수 없습니다.");

                if (workOrder.Status != "RUNNING")
                    throw new Exception("RUNNING 상태의 작업지시만 생산실적을 등록할 수 있습니다.");

                await _context.ProductionResults.AddAsync(result);
                await _context.SaveChangesAsync();

                var totalProduction = await _context.ProductionResults
                    .Where(x => x.WorkOrderId == workOrder.WorkOrderId)
                    .SumAsync(x => x.ProductionQuantity);

                if (totalProduction >= workOrder.TargetQuantity)
                {
                    workOrder.Status = "COMPLETED";
                    workOrder.EndTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
