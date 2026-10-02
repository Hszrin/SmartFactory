using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class ProductionPlanRepository : IProductionPlanRepository
    {
        private readonly SmartFactoryDbContext _context;

        public ProductionPlanRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductionPlan>> GetAllAsync(CancellationToken token)
        {
            return await _context.ProductionPlans
                .Include(x => x.Product)
                .Include(x => x.Line)
                .OrderBy(x => x.PlanId)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(ProductionPlan plan)
        {
            var target = await _context.ProductionPlans
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId);

            if (target == null)
                return;

            target.ProductId = plan.ProductId;
            target.LineId = plan.LineId;
            target.TargetQuantity = plan.TargetQuantity;
            target.StartDate = plan.StartDate;
            target.EndDate = plan.EndDate;
            target.Status = plan.Status;

            await _context.SaveChangesAsync();
        }
    }
}
