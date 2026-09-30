using Microsoft.EntityFrameworkCore;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;

namespace SmartFactory.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SmartFactoryDbContext _context;
        public ProductRepository(
            SmartFactoryDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 등록된 모든 제품 정보를 조회합니다.
        /// ProductId 순으로 정렬하여 반환합니다.
        /// </summary>
        public async Task<List<Product>> GetAllAsync(CancellationToken token)
        {
            return await _context.Products
                .OrderBy(x => x.ProductId)
                .ToListAsync(token);
        }

        /// <summary>
        /// 기존 제품 정보를 수정합니다.
        /// ProductId를 기준으로 DB에서 제품을 찾은 후
        /// 제품 코드, 제품명, 단위를 변경합니다.
        /// </summary>
        /// <param name="product">수정할 제품 정보</param>
        public async Task UpdateAsync(Product product)
        {
            var target = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.ProductId == product.ProductId);

            // 해당 ProductId가 DB에 존재하지 않는 경우
            if (target == null)
                return;

            target.ProductCode = product.ProductCode;
            target.ProductName = product.ProductName;
            target.Unit = product.Unit;

            await _context.SaveChangesAsync();
        }
    }
}