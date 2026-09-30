using SmartFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Services.Interface
{
    public interface IDefectService
    {
        Task AddDefect(Defect defect);
        Task UpdateDefect(Defect defect);
    }
}
