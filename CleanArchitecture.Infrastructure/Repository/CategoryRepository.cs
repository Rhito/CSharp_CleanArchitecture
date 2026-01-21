using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Category> AddAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
           bool  hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                InvalidOperationException ex = new InvalidOperationException("Cannot delete category because it has associated products.");
                throw ex;
            }

            var rowAffected = await _context.Categories
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();
              return rowAffected > 0;
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            var categories = await _context.Categories.Take(1000).AsNoTracking().ToListAsync();
            return categories;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id) ?? throw new KeyNotFoundException($"Category with id {id} not found.");
        }

        public async Task<Category> UpdateAsync(int id, Category category)
        {
            var rowAffected = await _context.Categories.Where(c => c.Id == id).ExecuteUpdateAsync(
                s => s.SetProperty(c => c.Name, category.Name)
                      .SetProperty(c => c.Description, category.Description));
            if(rowAffected == 0)
            {
                throw new KeyNotFoundException($"The id {id} is not valid.");
            }
            return category;
        }
    }
}
