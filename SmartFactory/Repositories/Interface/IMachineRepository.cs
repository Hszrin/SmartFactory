using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface IMachineRepository
    {
        Task<List<Machine>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(Machine machine);
    }
}