using SmartFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Repositories.Interface
{
    public interface IWorkOrderRepository
    {
        Task<List<WorkOrder>> GetAllAsync(CancellationToken token);
        Task UpdateAsync(WorkOrder workOrder);
    }
}
