using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories;
using SmartFactory.Repositories.Interface;
using SmartFactory.Services.Interface;

namespace SmartFactory.Services
{
    public class DefectService : IDefectService
    {
        private readonly IDefectRepository _defectRepository;
        private readonly SmartFactoryDbContext _context;
        public DefectService(
            IDefectRepository defectRepository,
            SmartFactoryDbContext context)
        {
            _defectRepository = defectRepository;
            _context = context;
        }
        public async Task AddDefect(Defect defect)
        {
            var result = await _context.ProductionResults
                .FirstOrDefaultAsync(x => x.ResultId == defect.ResultId);

            if (result == null)
                throw new Exception("생산 실적을 찾을 수 없습니다.");

            var currentTotal = await _context.Defects
                .Where(x => x.ResultId == defect.ResultId)
                .SumAsync(x => (int?)x.DefectQuantity) ?? 0;

            if (currentTotal + defect.DefectQuantity > result.DefectQuantity)
                throw new Exception("등록 가능한 불량 수량을 초과했습니다.");

            await _context.Defects.AddAsync(defect);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDefect(Defect defect)
        {
            var target = await _context.Defects
                .FirstOrDefaultAsync(x => x.DefectId == defect.DefectId);

            if (target == null)
                throw new Exception("불량 정보를 찾을 수 없습니다.");

            var result = await _context.ProductionResults
                .FirstOrDefaultAsync(x => x.ResultId == defect.ResultId);

            if (result == null)
                throw new Exception("생산 실적을 찾을 수 없습니다.");

            var otherTotal = await _context.Defects
                .Where(x =>
                    x.ResultId == defect.ResultId &&
                    x.DefectId != defect.DefectId)
                .SumAsync(x => (int?)x.DefectQuantity) ?? 0;

            if (otherTotal + defect.DefectQuantity > result.DefectQuantity)
                throw new Exception("등록 가능한 불량 수량을 초과했습니다.");

            await _defectRepository.UpdateAsync(defect);
        }
    }
}
