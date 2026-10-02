using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class DefectRepository : IDefectRepository
    {
        private readonly SmartFactoryDbContext _context;

        public DefectRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Defect>> GetAllAsync(CancellationToken token)
        {
            return await _context.Defects
                .Include(x => x.Result)
                    .ThenInclude(x => x!.WorkOrder)
                        .ThenInclude(x => x!.Product)
                .OrderBy(x => x.DefectId)
                .ToListAsync(token);
        }

        public async Task<List<Defect>> GetByResultIdAsync(long resultId)
        {
            return await _context.Defects
                .Where(x => x.ResultId == resultId)
                .OrderBy(x => x.DefectId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Defect defect)
        {
            var target = await _context.Defects
                .FirstOrDefaultAsync(x => x.DefectId == defect.DefectId);

            if (target == null)
                return;

            target.ResultId = defect.ResultId;
            target.DefectType = defect.DefectType;
            target.DefectQuantity = defect.DefectQuantity;
            target.Description = defect.Description;

            await _context.SaveChangesAsync();
        }
    }
}
