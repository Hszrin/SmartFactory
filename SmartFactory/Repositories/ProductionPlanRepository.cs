using SmartFactory.Data;
using SmartFactory.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Windows;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class ProductionPlanRepository : IProductionPlanRepository
    {
        private readonly SmartFactoryDbContext _context;
        public ProductionPlanRepository(
            SmartFactoryDbContext context)
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
                .FirstOrDefaultAsync(x =>
                    x.PlanId == plan.PlanId);

            // 해당 PlanId가 DB에 존재하지 않는 경우
            if (target == null)
                return;

            target.ProductId = plan.ProductId;
            target.PlanId = plan.PlanId;
            target.TargetQuantity = plan.TargetQuantity;
            target.StartDate = plan.StartDate;
            target.EndDate = plan.EndDate;
            target.Status = plan.Status;


            await _context.SaveChangesAsync();
        }
    }
}
