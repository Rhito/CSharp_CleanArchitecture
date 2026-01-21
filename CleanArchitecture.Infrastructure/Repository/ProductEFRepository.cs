using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class ProductEFRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductEFRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete)
        {
            var query = _context.Products.Where(p => ids.Contains(p.Id));

            if (isHardDelete)
                return await query.ExecuteDeleteAsync() > 0;

            return await query.ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)) > 0;
        }

        public async Task<bool> BulkRestoreAsync(List<int> ids)
        {
            var rowsAffected = await _context.Products
                .Where(p => ids.Contains(p.Id) && p.IsDeleted == true)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, false));

            return rowsAffected > 0;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().Take(10000).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id) ?? null!;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product); // Xóa hẳn khỏi bảng
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(ProductSearchFilter productSearchFilter)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking().Include(p => p.Category);

            // Apply filters
            if (!string.IsNullOrEmpty(productSearchFilter.Keyword))
            {
                query = query.Where(p => p.Name.Contains(productSearchFilter.Keyword)
                                            || (p.Description != null && p.Description.Contains(productSearchFilter.Keyword)));
            }

            // filter by category
            if (productSearchFilter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productSearchFilter.CategoryId.Value);

            // filter by soft delete status
            if (productSearchFilter.IsDeleted.HasValue)
                query = query.Where(p => p.IsDeleted == productSearchFilter.IsDeleted.Value);

            // filter by price range
            if (productSearchFilter.FromPrice.HasValue)
                query = query.Where(p => p.Price >= productSearchFilter.FromPrice.Value);
            if (productSearchFilter.ToPrice.HasValue)
                query = query.Where(p => p.Price <= productSearchFilter.ToPrice.Value);

            // Apply pagination 
            int skip = (productSearchFilter.PageNumber - 1) * productSearchFilter.PageSize;

            return await query.Skip(skip).Take(productSearchFilter.PageSize).ToListAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Product> UpdateAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null) throw new KeyNotFoundException($"The id {id} is not valid.");

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.IsDeleted = product.IsDeleted;
            await _context.SaveChangesAsync();
            return existingProduct;


            //var rowsAffected = await _context.Products
            //    .Where(p => p.Id == id)
            //    .ExecuteUpdateAsync(s => s
            //        .SetProperty(p => p.Name, product.Name)
            //        .SetProperty(p => p.Description, product.Description)
            //        .SetProperty(p => p.Price, product.Price)
            //        .SetProperty(p => p.StockQuantity, product.StockQuantity)
            //        .SetProperty(p => p.CategoryId, product.CategoryId)
            //        .SetProperty(p => p.IsDeleted, product.IsDeleted)
            //    );
            //if (rowsAffected == 0) throw new KeyNotFoundException($"The id {id} is not valid.");
            //return product;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            product.IsDeleted = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<(int Id, bool IsDeleted)>> GetListDeletedByIds(List<int> ids)
        {
            var productStatesRaw = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Id, p.IsDeleted }) // Sử dụng 'new' thay vì dùng ngoặc đơn Tuple
                .ToListAsync();

            // Sau khi đã có dữ liệu trong bộ nhớ, chúng ta chuyển sang Tuple
            return productStatesRaw.Select(x => (x.Id, x.IsDeleted ?? false)).ToList();
        }
    } 
}
