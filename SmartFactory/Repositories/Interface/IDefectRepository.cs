using SmartFactory.Models;

namespace SmartFactory.Repositories.Interface
{
    public interface IDefectRepository
    {
        Task<List<Defect>> GetAllAsync(CancellationToken token);
        Task<List<Defect>> GetByResultIdAsync(long resultId);
        Task UpdateAsync(Defect defect);
    }
}
