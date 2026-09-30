using SmartFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Repositories.Interface
{
    public interface IDefectRepository
    {
        Task<List<Defect>> GetAllAsync(CancellationToken token);
        Task<List<Defect>> GetByResultIdAsync(long resultId);
        Task UpdateAsync(Defect defect);
    }
}
