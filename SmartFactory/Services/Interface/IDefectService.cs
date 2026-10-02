using SmartFactory.Models;

namespace SmartFactory.Services.Interface
{
    public interface IDefectService
    {
        Task AddDefect(Defect defect);
        Task UpdateDefect(Defect defect);
    }
}
