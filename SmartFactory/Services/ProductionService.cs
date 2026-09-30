using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories;
using SmartFactory.Services.Interface;

namespace SmartFactory.Services
{
    public class ProductionService
    : IProductionService
    {
        private readonly SmartFactoryDbContext _context;

        public ProductionService(
            SmartFactoryDbContext context)
        {
            _context = context;
        }
        public async Task RegisterResultAsync(ProductionResult result)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. 작업지시 확인
                var workOrder = await _context.WorkOrders
                    .FirstOrDefaultAsync(
                        x => x.WorkOrderId == result.WorkOrderId);

                if (workOrder == null)
                    throw new Exception("작업지시를 찾을 수 없습니다.");

                if (workOrder.Status != "RUNNING")
                    throw new Exception(
                        "RUNNING 상태의 작업지시만 생산실적을 등록할 수 있습니다.");

                // 2. 생산실적 등록
                await _context.ProductionResults.AddAsync(result);

                await _context.SaveChangesAsync();

                // 3. 해당 작업지시의 누적 생산량 계산
                var totalProduction =
                    await _context.ProductionResults
                        .Where(x =>
                            x.WorkOrderId == workOrder.WorkOrderId)
                        .SumAsync(x =>
                            x.ProductionQuantity);

                // 4. 목표수량 도달 여부 확인
                if (totalProduction >= workOrder.TargetQuantity)
                {
                    workOrder.Status = "COMPLETED";
                    workOrder.EndTime = DateTime.Now;

                    await _context.SaveChangesAsync();
                }

                // 5. 전부 정상 처리되면 확정
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