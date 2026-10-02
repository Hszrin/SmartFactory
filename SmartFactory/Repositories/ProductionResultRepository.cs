using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class ProductionResultRepository : IProductionResultRepository
    {
        private readonly SmartFactoryDbContext _context;

        public ProductionResultRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductionResult>> GetAllAsync(CancellationToken token)
        {
            return await _context.ProductionResults
                .Include(x => x.WorkOrder)
                .Include(x => x.Machine)
                .OrderByDescending(x => x.ProductionTime)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(ProductionResult result)
        {
            var target = await _context.ProductionResults
                .FirstOrDefaultAsync(x => x.ResultId == result.ResultId);

            if (target == null)
                return;

            target.WorkOrderId = result.WorkOrderId;
            target.MachineId = result.MachineId;
            target.ProductionQuantity = result.ProductionQuantity;
            target.GoodQuantity = result.GoodQuantity;
            target.DefectQuantity = result.DefectQuantity;
            target.ProductionTime = result.ProductionTime;

            await _context.SaveChangesAsync();
        }
    }
}
