using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class LineRepository : ILineRepository
    {
        private readonly SmartFactoryDbContext _context;

        public LineRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductionLine>> GetAllAsync(CancellationToken token)
        {
            return await _context.ProductionLines
                .OrderBy(x => x.LineId)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(ProductionLine line)
        {
            var target = await _context.ProductionLines
                .FirstOrDefaultAsync(x => x.LineId == line.LineId);

            if (target == null)
                return;

            target.LineCode = line.LineCode;
            target.LineName = line.LineName;
            target.Status = line.Status;

            await _context.SaveChangesAsync();
        }
    }
}