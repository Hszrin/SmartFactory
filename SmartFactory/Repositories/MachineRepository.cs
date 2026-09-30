using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class MachineRepository : IMachineRepository
    {
        private readonly SmartFactoryDbContext _context;

        public MachineRepository(SmartFactoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Machine>> GetAllAsync(CancellationToken token)
        {
            return await _context.Machines
                .Include(x => x.Line)
                .OrderBy(x => x.MachineId)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(Machine machine)
        {
            var target = await _context.Machines
                .FirstOrDefaultAsync(x => x.MachineId == machine.MachineId);

            if (target == null)
                return;

            target.MachineCode = machine.MachineCode;
            target.MachineName = machine.MachineName;
            target.LineId = machine.LineId;
            target.Status = machine.Status;

            await _context.SaveChangesAsync();
        }
    }
}